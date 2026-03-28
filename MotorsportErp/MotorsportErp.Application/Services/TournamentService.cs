using MotorsportErp.Application.DTO.Tournaments;
using MotorsportErp.Application.Interfaces.Repositories;
using MotorsportErp.Application.Interfaces.Services;
using MotorsportErp.Application.Mappers;
using MotorsportErp.Domain.Tournaments;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.Services;

public class TournamentService : ITournamentService
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICarRepository _carRepository;
    private readonly ITournamentApplicationRepository _applicationRepository;

    public TournamentService(
        ITournamentRepository tournamentRepository,
        IUserRepository userRepository,
        ICarRepository carRepository,
        ITournamentApplicationRepository applicationRepository)
    {
        _tournamentRepository = tournamentRepository;
        _userRepository = userRepository;
        _carRepository = carRepository;
        _applicationRepository = applicationRepository;
    }

    public async Task<Guid> CreateAsync(Guid userId, TournamentCreateRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        user.Roles |= UserRole.Organizer;

        var tournament = new Tournament
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            TrackId = request.TrackId,
            StartDate = request.StartDate,
            RequiredParticipants = request.RequiredParticipants,
            AllowedCarClass = request.AllowedCarClass,
            RequiredRaceCount = request.RequiredRaceCount,
            Status = TournamentStatus.RegistrationOpen,
            CreatorId = userId,
            Organizers =
            [
                new TournamentOrganizer
                {
                    Id = Guid.NewGuid(),
                    UserId = userId
                }
            ]
        };

        await _tournamentRepository.AddAsync(tournament);
        await _userRepository.UpdateAsync(user);

        return tournament.Id;
    }

    public async Task UpdateAsync(Guid userId, Guid tournamentId, TournamentUpdateRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId)
            ?? throw new KeyNotFoundException("Tournament not found");

        if (!HasAccess(tournament, user))
        {
            throw new UnauthorizedAccessException("No permission");
        }

        if (tournament.Status != TournamentStatus.RegistrationOpen)
        {
            throw new InvalidOperationException("Cannot update after registration");
        }

        tournament.Description = request.Description;
        tournament.StartDate = request.StartDate;
        tournament.RequiredParticipants = request.RequiredParticipants;

        await _tournamentRepository.UpdateAsync(tournament);
    }

    public async Task CancelAsync(Guid userId, Guid tournamentId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId)
            ?? throw new KeyNotFoundException("Tournament not found");

        if (!HasAccess(tournament, user))
        {
            throw new UnauthorizedAccessException("No permission");
        }

        if (tournament.Status == TournamentStatus.Finished)
        {
            throw new InvalidOperationException("Cannot cancel finished tournament");
        }

        tournament.Status = TournamentStatus.Cancelled;

        await _tournamentRepository.UpdateAsync(tournament);
    }

    public async Task ApplyAsync(Guid userId, Guid tournamentId, Guid carId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId)
            ?? throw new KeyNotFoundException("Tournament not found");

        var car = await _carRepository.GetByIdAsync(carId)
            ?? throw new KeyNotFoundException("Car not found");

        if (tournament.Status != TournamentStatus.RegistrationOpen)
        {
            throw new InvalidOperationException("Registration closed");
        }

        if (car.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("Not your car");
        }

        if (car.CarClass != tournament.AllowedCarClass)
        {
            throw new InvalidOperationException("Car class mismatch");
        }

        if (user.RaceCount < tournament.RequiredRaceCount)
        {
            throw new InvalidOperationException("Not enough races");
        }

        bool exists = await _applicationRepository.ExistsAsync(userId, tournamentId);
        if (exists)
        {
            throw new InvalidOperationException("Already applied");
        }

        var application = new TournamentApplication
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TournamentId = tournamentId,
            CarId = carId,
            Status = TournamentApplicationStatus.Pending
        };

        await _applicationRepository.AddAsync(application);
    }

    public async Task ApproveAsync(Guid userId, Guid applicationId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        var application = await _applicationRepository.GetByIdAsync(applicationId)
            ?? throw new KeyNotFoundException("Application not found");

        var tournament = await _tournamentRepository.GetByIdAsync(application.TournamentId)
            ?? throw new KeyNotFoundException("Tournament not found");

        if (!HasAccess(tournament, user))
        {
            throw new UnauthorizedAccessException("No permission");
        }

        application.Status = TournamentApplicationStatus.Approved;

        var approvedCount = await _applicationRepository.GetApprovedCountAsync(tournament.Id);

        if (approvedCount >= tournament.RequiredParticipants)
        {
            tournament.Status = TournamentStatus.Confirmed;
            await _tournamentRepository.UpdateAsync(tournament);
        }

        await _applicationRepository.UpdateAsync(application);
    }

    public async Task RejectAsync(Guid userId, Guid applicationId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        var application = await _applicationRepository.GetByIdAsync(applicationId)
            ?? throw new KeyNotFoundException("Application not found");

        var tournament = await _tournamentRepository.GetByIdAsync(application.TournamentId)
            ?? throw new KeyNotFoundException("Tournament not found");

        if (!HasAccess(tournament, user))
        {
            throw new UnauthorizedAccessException("No permission");
        }

        application.Status = TournamentApplicationStatus.Rejected;

        await _applicationRepository.UpdateAsync(application);
    }

    public async Task StartAsync(Guid userId, Guid tournamentId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId)
            ?? throw new KeyNotFoundException("Tournament not found");

        if (!HasAccess(tournament, user))
        {
            throw new UnauthorizedAccessException("No permission");
        }

        if (tournament.Status != TournamentStatus.Confirmed)
        {
            throw new InvalidOperationException("Tournament not confirmed");
        }

        if (DateTime.UtcNow < tournament.StartDate)
        {
            throw new InvalidOperationException("Too early to start");
        }

        tournament.Status = TournamentStatus.Active;

        await _tournamentRepository.UpdateAsync(tournament);
    }

    public async Task FinishAsync(Guid userId, Guid tournamentId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId)
            ?? throw new KeyNotFoundException("Tournament not found");

        if (!HasAccess(tournament, user))
        {
            throw new UnauthorizedAccessException("No permission");
        }

        if (tournament.Status != TournamentStatus.Active)
        {
            throw new InvalidOperationException("Tournament not active");
        }

        if (!tournament.Results.Any())
        {
            throw new InvalidOperationException("No results");
        }

        tournament.Status = TournamentStatus.Finished;

        await _tournamentRepository.UpdateAsync(tournament);
    }

    public async Task AddResultAsync(Guid userId, Guid tournamentId, TournamentResultRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId)
            ?? throw new KeyNotFoundException("Tournament not found");

        if (!HasAccess(tournament, user))
        {
            throw new UnauthorizedAccessException("No permission");
        }

        if (tournament.Status != TournamentStatus.Active)
        {
            throw new InvalidOperationException("Tournament not active");
        }

        if (tournament.Results.Any(r => r.UserId == request.UserId))
        {
            throw new InvalidOperationException("Result already exists");
        }

        if (request.Position <= 0)
        {
            throw new ArgumentException("Invalid position");
        }

        if (request.Points < 0)
        {
            throw new ArgumentException("Invalid points");
        }

        tournament.Results.Add(new TournamentResult
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Position = request.Position
        });

        await _tournamentRepository.UpdateAsync(tournament);
    }

    public async Task<List<TournamentResponse>> GetAllAsync()
    {
        var tournaments = await _tournamentRepository.GetAllAsync();
        return tournaments.Select(TournamentMapper.ToResponse).ToList();
    }

    public async Task<TournamentDetailsResponse> GetByIdAsync(Guid id)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Tournament not found");

        return TournamentMapper.ToDetails(tournament);
    }

    private static bool HasAccess(Tournament tournament, User user)
    {
        return tournament.CreatorId == user.Id ||
               tournament.Organizers.Any(o => o.UserId == user.Id) ||
               user.Roles.HasFlag(UserRole.Moderator) ||
               user.Roles.HasFlag(UserRole.SuperAdmin);
    }
}