using Microsoft.Extensions.Options;
using MotorsportErp.Application.Common.Settings;
using MotorsportErp.Application.DTO.Tournaments;
using MotorsportErp.Application.Interfaces.Repositories;
using MotorsportErp.Application.Interfaces.Services;
using MotorsportErp.Application.Mappers;
using MotorsportErp.Domain.Tournaments;
using MotorsportErp.Domain.Tracks;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.Services;

public class TournamentService : ITournamentService
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly ITournamentApplicationRepository _tournamentApplicationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITrackRepository _trackRepository;
    private readonly ICarRepository _carRepository;
    private readonly TournamentSettings _settings;

    public TournamentService(
        ITournamentRepository tournamentRepository,
        IUserRepository userRepository,
        ITrackRepository trackRepository,
        ICarRepository carRepository,
        ITournamentApplicationRepository tournamentApplicationRepository,
        IOptions<TournamentSettings> options)
    {
        _tournamentRepository = tournamentRepository;
        _userRepository = userRepository;
        _trackRepository = trackRepository;
        _carRepository = carRepository;
        _tournamentApplicationRepository = tournamentApplicationRepository;
        _settings = options.Value;
    }

    public async Task<List<TournamentResponse>> GetAllAsync()
    {
        List<Tournament> tournaments = await _tournamentRepository.GetAllAsync();
        return TournamentMapper.ToResponseList(tournaments).ToList();
    }

    public async Task<TournamentDetailsResponse> GetByIdAsync(Guid id)
    {
        Tournament tournament = await _tournamentRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Tournament not found");
        return TournamentMapper.ToDetails(tournament);
    }

    public async Task<Guid> CreateAsync(Guid userId, TournamentCreateRequest request)
    {
        User user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        if (user.RaceCount < _settings.MinRacesToBecomeOrganizer)
        {
            throw new InvalidOperationException("Not enough races to create tournament");
        }

        Track track = await _trackRepository.GetByIdAsync(request.TrackId) ?? throw new KeyNotFoundException("Track not found");
        if (track.Status is not TrackStatus.Confirmed and not TrackStatus.Official)
        {
            throw new InvalidOperationException("Track is not available for tournaments");
        }

        Tournament tournament = TournamentMapper.ToEntity(request, userId);
        await _tournamentRepository.AddAsync(tournament);

        if (!user.Roles.HasFlag(UserRole.Organizer))
        {
            user.Roles |= UserRole.Organizer;
            await _userRepository.UpdateAsync(user);
        }

        return tournament.Id;
    }

    public async Task UpdateAsync(Guid tournamentId, TournamentUpdateRequest request)
    {
        Tournament tournament = await _tournamentRepository.GetByIdAsync(tournamentId) ?? throw new KeyNotFoundException("Tournament not found");
        if (tournament.Status is TournamentStatus.Finished or TournamentStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot update completed tournament");
        }

        TournamentMapper.Update(tournament, request);

        await _tournamentRepository.UpdateAsync(tournament);
    }

    public async Task ApplyAsync(Guid userId, TournamentApplyRequest request)
    {
        User user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        Tournament tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId) ?? throw new KeyNotFoundException("Tournament not found");
        Domain.Cars.Car car = await _carRepository.GetByIdAsync(request.CarId) ?? throw new KeyNotFoundException("Car not found");

        if (tournament.Status != TournamentStatus.RegistrationOpen)
        {
            throw new InvalidOperationException("Tournament registration is closed");
        }

        if (car.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("Car does not belong to user");
        }

        if (car.CarClass != tournament.AllowedCarClass)
        {
            throw new InvalidOperationException("Car class does not match tournament requirements");
        }

        if (user.RaceCount < tournament.RequiredRaceCount)
        {
            throw new InvalidOperationException("Not enough races to participate");
        }

        bool alreadyApplied = await _tournamentRepository.HasUserAppliedAsync(tournament.Id, userId);
        if (alreadyApplied)
        {
            throw new InvalidOperationException("User already applied to this tournament");
        }

        TournamentApplication application = new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TournamentId = tournament.Id,
            CarId = car.Id,
            Status = TournamentApplicationStatus.Pending
        };

        await _tournamentRepository.AddApplicationAsync(application);
    }

    public async Task ApproveApplicationAsync(Guid userId, Guid applicationId)
    {
        var application = await _tournamentRepository.GetApplicationByIdAsync(applicationId) ?? throw new KeyNotFoundException("Application not found");
        var tournament = application.Tournament;

        if (!IsUserOrganizer(tournament, userId))
        {
            throw new UnauthorizedAccessException("Only organizer can approve applications");
        }

        if (tournament.Status != TournamentStatus.RegistrationOpen)
        {
            throw new InvalidOperationException("Tournament is not accepting applications");
        }

        if (application.Status != TournamentApplicationStatus.Pending)
        {
            throw new InvalidOperationException("Application is not pending");
        }

        application.Status = TournamentApplicationStatus.Approved;

        await _tournamentApplicationRepository.UpdateAsync(application);

        int approvedCount = tournament.Applications.Count(a => a.Status == TournamentApplicationStatus.Approved);

        if (approvedCount >= tournament.RequiredParticipants)
        {
            tournament.Status = TournamentStatus.Confirmed;
            await _tournamentRepository.UpdateAsync(tournament);
        }
    }

    public async Task RejectApplicationAsync(Guid userId, Guid applicationId)
    {
        var application = await _tournamentRepository.GetApplicationByIdAsync(applicationId) ?? throw new KeyNotFoundException("Application not found");
        var tournament = application.Tournament;

        if (!IsUserOrganizer(tournament, userId))
        {
            throw new UnauthorizedAccessException("Only organizer can reject applications");
        }

        if (application.Status != TournamentApplicationStatus.Pending)
        {
            throw new InvalidOperationException("Application is not pending");
        }

        application.Status = TournamentApplicationStatus.Rejected;

        await _tournamentApplicationRepository.UpdateAsync(application);
    }

    public async Task AddResultAsync(Guid userId, Guid tournamentId, TournamentResultCreateRequest request)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId) ?? throw new KeyNotFoundException("Tournament not found");
        
        if (!IsUserOrganizer(tournament, userId))
        {
            throw new UnauthorizedAccessException("Only organizer can add results");
        }

        if (tournament.Status is not TournamentStatus.Active and
            not TournamentStatus.Finished)
        {
            throw new InvalidOperationException("Tournament is not active");
        }

        bool participated = tournament.Applications.Any(a =>
            a.UserId == request.UserId &&
            a.Status == TournamentApplicationStatus.Approved);

        if (!participated)
        {
            throw new InvalidOperationException("User did not participate");
        }

        bool alreadyExists = tournament.Results.Any(r => r.UserId == request.UserId);
        if (alreadyExists)
        {
            throw new InvalidOperationException("Result already exists");
        }

        TournamentResult result = new()
        {
            Id = Guid.NewGuid(),
            TournamentId = tournamentId,
            UserId = request.UserId,
            Position = request.Position,
            BestLapTime = request.BestLapTime
        };

        await _tournamentRepository.AddResultAsync(result);
    }

    public async Task StartTournamentAsync(Guid userId, Guid tournamentId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId) ?? throw new KeyNotFoundException("Tournament not found");
        
        if (!IsUserOrganizer(tournament, userId))
        {
            throw new UnauthorizedAccessException();
        }

        if (tournament.Status != TournamentStatus.Confirmed)
        {
            throw new InvalidOperationException("Tournament must be confirmed");
        }

        tournament.Status = TournamentStatus.Active;

        await _tournamentRepository.UpdateAsync(tournament);
    }

    public async Task FinishTournamentAsync(Guid userId, Guid tournamentId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId) ?? throw new KeyNotFoundException("Tournament not found");
        
        if (!IsUserOrganizer(tournament, userId))
        {
            throw new UnauthorizedAccessException();
        }

        if (tournament.Status != TournamentStatus.Active)
        {
            throw new InvalidOperationException("Tournament is not active");
        }

        tournament.Status = TournamentStatus.Finished;

        await _tournamentRepository.UpdateAsync(tournament);
    }

    private static bool IsUserOrganizer(Tournament tournament, Guid userId)
    {
        return tournament.CreatorId == userId ||
               tournament.Organizers.Any(o => o.UserId == userId);
    }
}