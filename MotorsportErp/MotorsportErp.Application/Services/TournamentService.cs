using Microsoft.Extensions.Options;
using MotorsportErp.Application.Common.Settings;
using MotorsportErp.Application.DTO.Common;
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
    private readonly ITrackRepository _trackRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICarRepository _carRepository;
    private readonly ITournamentApplicationRepository _applicationRepository;
    private readonly IFileRepository _fileRepository;

    private readonly TournamentSettings _settings;

    public TournamentService(
        ITournamentRepository tournamentRepository,
        ITrackRepository trackRepository,
        IUserRepository userRepository,
        ICarRepository carRepository,
        ITournamentApplicationRepository applicationRepository,
        IOptions<TournamentSettings> options,
        IFileRepository fileRepository)
    {
        _tournamentRepository = tournamentRepository;
        _trackRepository = trackRepository;
        _userRepository = userRepository;
        _carRepository = carRepository;
        _applicationRepository = applicationRepository;
        _settings = options.Value;
        _fileRepository = fileRepository;
    }

    public async Task<Guid> CreateAsync(Guid userId, TournamentCreateRequest request)
    {
        if (request.StartDate >= request.EndDate)
        {
            throw new ArgumentException("Tournament end date must be after the start date.");
        }

        if (request.RequiredParticipants <= 0)
        {
            throw new ArgumentException("Required participants must be greater than zero.");
        }

        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        var track = await _trackRepository.GetByIdAsync(request.TrackId) ?? throw new KeyNotFoundException("Track not found");

        bool isPrivileged = user.Roles.HasFlag(UserRole.Organizer) ||
                        user.Roles.HasFlag(UserRole.Moderator) ||
                        user.Roles.HasFlag(UserRole.SuperAdmin);

        if (!isPrivileged)
        {
            if (user.RaceCount < _settings.MinRacesToBecomeOrganizer)
            {
                throw new UnauthorizedAccessException($"You need at least {_settings.MinRacesToBecomeOrganizer} races to create a tournament.");
            }

            user.Roles |= UserRole.Organizer;
        }

        if (track.Status == MotorsportErp.Domain.Tracks.TrackStatus.Unofficial)
        {
            throw new InvalidOperationException("Tournament can be created only on confirmed or official tracks.");
        }

        var tournament = new Tournament
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            TrackId = request.TrackId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
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
        if (request.StartDate >= request.EndDate)
        {
            throw new ArgumentException("Tournament end date must be after the start date.");
        }

        if (request.RequiredParticipants <= 0)
        {
            throw new ArgumentException("Required participants must be greater than zero.");
        }

        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId) ?? throw new KeyNotFoundException("Tournament not found");
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
        tournament.EndDate = request.EndDate;
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

        if (user.IsBlocked)
        {
            throw new UnauthorizedAccessException("User is blocked");
        }

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
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        var application = await _applicationRepository.GetByIdAsync(applicationId) ?? throw new KeyNotFoundException("Application not found");
        var tournament = await _tournamentRepository.GetByIdAsync(application.TournamentId) ?? throw new KeyNotFoundException("Tournament not found");

        if (!HasAccess(tournament, user))
        {
            throw new UnauthorizedAccessException("Only organizers can approve applications.");
        }

        if (application.Status != TournamentApplicationStatus.Pending)
        {
            throw new InvalidOperationException("Application is not pending");
        }

        if (tournament.Status != TournamentStatus.RegistrationOpen)
        {
            throw new InvalidOperationException("Applications can only be approved while registration is open.");
        }

        await _tournamentRepository.ApproveApplicationAtomicallyAsync(applicationId);
    }

    public async Task RejectAsync(Guid userId, Guid applicationId)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        var application = await _applicationRepository.GetByIdAsync(applicationId) ?? throw new KeyNotFoundException("Application not found");
        var tournament = await _tournamentRepository.GetByIdAsync(application.TournamentId) ?? throw new KeyNotFoundException("Tournament not found");

        if (!HasAccess(tournament, user))
        {
            throw new UnauthorizedAccessException("No permission");
        }

        if (application.Status != TournamentApplicationStatus.Pending)
        {
            throw new InvalidOperationException("Application is not pending");
        }

        if (tournament.Status != TournamentStatus.RegistrationOpen)
        {
            throw new InvalidOperationException("Applications can only be rejected while registration is open.");
        }

        application.Status = TournamentApplicationStatus.Rejected;

        await _applicationRepository.UpdateAsync(application);
    }

    public async Task StartAsync(Guid userId, Guid tournamentId)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId) ?? throw new KeyNotFoundException("Tournament not found");

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
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId) ?? throw new KeyNotFoundException("Tournament not found");

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
            throw new InvalidOperationException("No results to finish the tournament");
        }

        tournament.Status = TournamentStatus.Finished;
        await _tournamentRepository.UpdateAsync(tournament);

        foreach (var result in tournament.Results)
        {
            var participant = await _userRepository.GetByIdAsync(result.UserId);
            if (participant != null)
            {
                participant.RaceCount += 1;
                await _userRepository.UpdateAsync(participant);
            }
        }
    }

    public async Task AddResultAsync(Guid userId, Guid tournamentId, TournamentResultRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId) ?? throw new KeyNotFoundException("Tournament not found");

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

        var approvedApplication = tournament.Applications
            .Any(a => a.UserId == request.UserId && a.Status == TournamentApplicationStatus.Approved);

        if (!approvedApplication)
        {
            throw new InvalidOperationException("Result can be added only for approved participants.");
        }

        if (request.Position <= 0)
        {
            throw new ArgumentException("Invalid position");
        }

        tournament.Results.Add(new TournamentResult
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Position = request.Position,
            BestLapTime = request.BestLapTime
        });

        await _tournamentRepository.UpdateAsync(tournament);
    }

    public async Task<PagedResponse<TournamentResponse>> GetAllAsync(
        TournamentListQuery query,
        int page = 0,
        int pageSize = 20)
    {
        query ??= new TournamentListQuery();
        var (tournaments, totalCount) = await _tournamentRepository.GetFilteredPagedAsync(query, page, pageSize);

        return new PagedResponse<TournamentResponse>
        {
            Items = tournaments.Select(t => TournamentMapper.ToResponse(t)).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<TournamentDetailsResponse> GetByIdAsync(Guid id)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Tournament not found");

        return TournamentMapper.ToDetails(tournament);
    }

    public async Task<IReadOnlyCollection<TournamentApplicationResponse>> GetApplicationsAsync(Guid userId, Guid tournamentId)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId) ?? throw new KeyNotFoundException("Tournament not found");

        return !HasAccess(tournament, user)
            ? throw new UnauthorizedAccessException("No permission")
            : (IReadOnlyCollection<TournamentApplicationResponse>)tournament.Applications
            .OrderBy(a => a.Status)
            .ThenBy(a => a.User.Nickname)
            .Select(TournamentMapper.ToApplicationResponse)
            .ToList();
    }

    public async Task DeleteAsync(Guid userId, Guid tournamentId)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId) ?? throw new KeyNotFoundException("Tournament not found");

        bool isModerator = user.Roles.HasFlag(UserRole.Moderator) || user.Roles.HasFlag(UserRole.SuperAdmin);

        if (!isModerator)
        {
            throw new UnauthorizedAccessException("Only moderators can delete tournaments");
        }

        await _tournamentRepository.DeleteAsync(tournament);
    }

    public async Task AddOrganizerAsync(Guid userId, Guid tournamentId, Guid newOrganizerId)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId) ?? throw new KeyNotFoundException("Tournament not found");

        if (!HasAccess(tournament, user))
        {
            throw new UnauthorizedAccessException("No permission");
        }

        var newOrganizer = await _userRepository.GetByIdAsync(newOrganizerId) ?? throw new KeyNotFoundException("New organizer not found");

        if (tournament.Organizers.Any(o => o.UserId == newOrganizerId))
        {
            throw new InvalidOperationException("User is already an organizer");
        }

        tournament.Organizers.Add(new TournamentOrganizer
        {
            Id = Guid.NewGuid(),
            TournamentId = tournamentId,
            UserId = newOrganizerId
        });

        if (!newOrganizer.Roles.HasFlag(UserRole.Organizer))
        {
            newOrganizer.Roles |= UserRole.Organizer;
            await _userRepository.UpdateAsync(newOrganizer);
        }

        await _tournamentRepository.UpdateAsync(tournament);
    }

    public async Task CancelApplicationAsync(Guid userId, Guid applicationId)
    {
        var application = await _applicationRepository.GetByIdAsync(applicationId) ?? throw new KeyNotFoundException("Application not found");
        if (application.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only cancel your own applications");
        }

        var tournament = await _tournamentRepository.GetByIdAsync(application.TournamentId) ?? throw new KeyNotFoundException("Tournament not found");
        if (tournament.Status != TournamentStatus.RegistrationOpen)
        {
            throw new InvalidOperationException("Cannot cancel application after registration is closed");
        }

        var wasApproved = application.Status == TournamentApplicationStatus.Approved;
        application.Status = TournamentApplicationStatus.Cancelled;
        await _applicationRepository.UpdateAsync(application);

        if (wasApproved)
        {
            var approvedCount = await _applicationRepository.GetApprovedCountAsync(tournament.Id);
            if (approvedCount < tournament.RequiredParticipants && tournament.Status == TournamentStatus.Confirmed)
            {
                tournament.Status = TournamentStatus.RegistrationOpen;
                await _tournamentRepository.UpdateAsync(tournament);
            }
        }
    }

    public async Task AddPhotoAsync(Guid userId, Guid targetEntityId, Guid photoId)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        var tournament = await _tournamentRepository.GetByIdAsync(targetEntityId) ?? throw new KeyNotFoundException("Tournament not found");

        if (!HasAccess(tournament, user))
        {
            throw new UnauthorizedAccessException("No permission");
        }

        if (tournament.Status != TournamentStatus.RegistrationOpen)
        {
            throw new InvalidOperationException("Cannot update after registration");
        }

        var photo = await _fileRepository.GetByIdAsync(photoId) ?? throw new KeyNotFoundException("Photo not found");
        tournament.Photos.Add(photo);
        await _tournamentRepository.UpdateAsync(tournament);
    }

    public async Task RemovePhotoAsync(Guid userId, Guid targetEntityId, Guid photoId)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        var tournament = await _tournamentRepository.GetByIdAsync(targetEntityId) ?? throw new KeyNotFoundException("Tournament not found");

        if (!HasAccess(tournament, user))
        {
            throw new UnauthorizedAccessException("No permission");
        }

        if (tournament.Status != TournamentStatus.RegistrationOpen)
        {
            throw new InvalidOperationException("Cannot update after registration");
        }

        var photo = await _fileRepository.GetByIdAsync(photoId) ?? throw new KeyNotFoundException("Photo not found");
        if (photo != null)
        {
            _ = tournament.Photos.Remove(photo);
            await _tournamentRepository.UpdateAsync(tournament);
        }
    }

    private static bool HasAccess(Tournament tournament, User user)
    {
        return tournament.CreatorId == user.Id ||
               tournament.Organizers.Any(o => o.UserId == user.Id) ||
               user.Roles.HasFlag(UserRole.Moderator) ||
               user.Roles.HasFlag(UserRole.SuperAdmin);
    }
}
