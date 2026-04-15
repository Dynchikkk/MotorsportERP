using Microsoft.IdentityModel.Tokens.Experimental;
using MotorsportErp.Application.Common.Contracts;
using MotorsportErp.Application.Common.Exceptions;
using MotorsportErp.Application.Common.Extensions;
using MotorsportErp.Application.Common.Interfaces.Repositories;
using MotorsportErp.Application.Features.Cars.Mappers;
using MotorsportErp.Application.Features.Tournaments.Mappers;
using MotorsportErp.Application.Features.Users.Contracts;
using MotorsportErp.Application.Features.Users.Interfaces;
using MotorsportErp.Application.Features.Users.Mappers;
using MotorsportErp.Domain.Files;
using MotorsportErp.Domain.Tournaments;
using MotorsportErp.Domain.Tracks;
using MotorsportErp.Domain.Users;
using System.Linq.Expressions;

namespace MotorsportErp.Application.Features.Users.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IFileRepository _fileRepository;

    public UserService(IUserRepository userRepository, IFileRepository fileRepository)
    {
        _userRepository = userRepository;
        _fileRepository = fileRepository;
    }

    public async Task<UserResponse> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");
        return UserMapper.ToResponse(user);
    }

    public async Task<PublicUserProfileResponse> GetPublicProfileAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");
        return BuildPublicProfile(user);
    }

    public async Task<UserProfileResponse> GetProfileAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");
        return BuildProfile(user);
    }

    public async Task<PagedResponse<UserResponse>> GetAllAsync(string? search, int page, int pageSize)
    {
        Expression<Func<User, bool>>? filter = null;

        if (!string.IsNullOrWhiteSpace(search))
        {
            filter = u => u.Nickname.Contains(search);
        }

        var (users, totalCount) = await _userRepository.GetPagedAsync(filter, page, pageSize);

        return new PagedResponse<UserResponse>
        {
            Items = users.Select(UserMapper.ToResponse).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedResponse<UserAdminResponse>> GetAllAdminAsync(int page, int pageSize)
    {
        var (users, totalCount) = await _userRepository.GetPagedAsync(null, page, pageSize);

        return new PagedResponse<UserAdminResponse>
        {
            Items = users.Select(UserMapper.ToAdminResponse).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task AssignRoleAsync(Guid adminId, Guid targetUserId, UserRole role)
    {
        if (role == UserRole.SuperAdmin)
        {
            throw new InvalidOperationException("Cannot assign SuperAdmin role");
        }

        var admin = await _userRepository.GetByIdAsync(adminId) ?? throw new KeyNotFoundException("Admin not found");
        if (!admin.Roles.HasFlag(UserRole.SuperAdmin))
        {
            throw new ForbiddenException("Only super admin can assign roles");
        }

        var user = await _userRepository.GetByIdAsync(targetUserId) ?? throw new KeyNotFoundException("User not found");
        if (user.Roles.HasFlag(role))
        {
            throw new InvalidOperationException("User already has this role");
        }

        user.Roles |= role;

        await _userRepository.UpdateAsync(user);
    }

    public async Task UpdateProfileAsync(Guid userId, UserUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nickname))
        {
            throw new ArgumentException("Nickname cannot be empty");
        }

        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");

        user.Nickname = request.Nickname;
        user.Bio = request.Bio;

        MediaFile? file = request.Avatar != null ?
            await _fileRepository.GetByIdAsync(request.Avatar.Id) ?? throw new KeyNotFoundException("Photo not found") :
            null;
        if (file != null && file.UploadedById != userId)
        {
            throw new ForbiddenException("Only owner can use self photos");
        }

        user.Avatar = file;

        await _userRepository.UpdateAsync(user);
    }

    public async Task BlockUserAsync(Guid adminId, Guid targetUserId, bool block)
    {
        var admin = await _userRepository.GetByIdAsync(adminId) ?? throw new KeyNotFoundException("Admin not found");
        if (!admin.Roles.HasFlag(UserRole.Moderator) && !admin.Roles.HasFlag(UserRole.SuperAdmin))
        {
            throw new ForbiddenException("No permission to block users");
        }

        var targetUser = await _userRepository.GetByIdAsync(targetUserId) ?? throw new KeyNotFoundException("User not found");
        if (targetUser.Roles.HasFlag(UserRole.Moderator) || targetUser.Roles.HasFlag(UserRole.SuperAdmin))
        {
            throw new InvalidOperationException("Can't block user with role Moderator or SuperAdmin");
        }

        targetUser.IsBlocked = block;
        await _userRepository.UpdateAsync(targetUser);
    }

    private UserProfileResponse BuildProfile(User user)
    {
        var publicProfile = BuildPublicProfile(user);
        var response = UserMapper.ToProfile(user);

        response.CarsCount = publicProfile.CarsCount;
        response.TournamentsCount = publicProfile.TournamentsCount;
        response.Cars = publicProfile.Cars;
        response.CurrentTournaments = publicProfile.CurrentTournaments;
        response.TournamentHistory = publicProfile.TournamentHistory;
        response.Applications = BuildApplications(user);
        response.OrganizedTournaments = user.OrganizedTournaments
            .Where(o => o.Tournament != null)
            .OrderByDescending(o => o.Tournament.StartDate)
            .Select(o => TournamentMapper.ToResponse(o.Tournament))
            .ToList();

        return response;
    }

    private PublicUserProfileResponse BuildPublicProfile(User user)
    {
        var response = UserMapper.ToPublicProfile(user);
        var resultLookup = user.Results
            .Where(r => r.Tournament != null)
            .GroupBy(r => r.TournamentId)
            .ToDictionary(g => g.Key, g => g.OrderBy(r => r.Position).First());

        response.Cars = user.Cars
            .OrderByDescending(c => c.Year)
            .ThenBy(c => c.Brand)
            .ThenBy(c => c.Model)
            .Select(CarMapper.ToResponse)
            .ToList();

        response.CurrentTournaments = user.Applications
            .Where(IsCurrentParticipation)
            .OrderBy(a => a.Tournament!.StartDate)
            .Select(a => MapTournamentEntry(a, resultLookup))
            .ToList();

        response.TournamentHistory = user.Applications
            .Where(IsHistoryParticipation)
            .OrderByDescending(a => a.Tournament!.EndDate)
            .Select(a => MapTournamentEntry(a, resultLookup))
            .ToList();

        response.CarsCount = response.Cars.Count;
        response.TournamentsCount = response.CurrentTournaments.Count + response.TournamentHistory.Count;

        return response;
    }

    private static List<UserTournamentEntryResponse> BuildApplications(User user)
    {
        var resultLookup = user.Results
            .Where(r => r.Tournament != null)
            .GroupBy(r => r.TournamentId)
            .ToDictionary(g => g.Key, g => g.OrderBy(r => r.Position).First());

        return user.Applications
            .Where(a => a.Tournament != null && a.Car != null)
            .OrderByDescending(a => a.Tournament!.StartDate)
            .Select(a => MapTournamentEntry(a, resultLookup))
            .ToList();
    }

    private static bool IsCurrentParticipation(TournamentApplication application)
    {
        return application.Tournament != null &&
               application.Car != null &&
               application.Status == TournamentApplicationStatus.Approved &&
               application.Tournament.Status != TournamentStatus.Finished &&
               application.Tournament.Status != TournamentStatus.Cancelled;
    }

    private static bool IsHistoryParticipation(TournamentApplication application)
    {
        return application.Tournament != null &&
               application.Car != null &&
               application.Status == TournamentApplicationStatus.Approved &&
               (application.Tournament.Status == TournamentStatus.Finished ||
                application.Tournament.Status == TournamentStatus.Cancelled);
    }

    private static UserTournamentEntryResponse MapTournamentEntry(
        TournamentApplication application,
        IReadOnlyDictionary<Guid, TournamentResult> resultLookup)
    {
        var result = resultLookup.GetValueOrDefault(application.TournamentId);

        return new UserTournamentEntryResponse
        {
            ApplicationId = application.Id,
            TournamentId = application.TournamentId,
            TournamentName = application.Tournament!.Name,
            TournamentStatus = application.Tournament.Status,
            StartDate = application.Tournament.StartDate,
            EndDate = application.Tournament.EndDate,
            TrackId = application.Tournament.TrackId,
            TrackName = application.Tournament.Track?.Name ?? string.Empty,
            CarId = application.CarId,
            CarName = $"{application.Car!.Brand} {application.Car.Model}",
            ApplicationStatus = application.Status,
            Position = result?.Position,
            BestLapTime = result?.BestLapTime
        };
    }

    public UserReferenceDataResponce GetReferenceData()
    {
        var invisibleRoles = new List<string>() { UserRole.Moderator.ToString(), UserRole.SuperAdmin.ToString() };
        return new UserReferenceDataResponce
        {
            UserRoles = EnumExtensions.GetEnumValues<TrackStatus>()
                 .Where(item => !invisibleRoles.Contains(item.Key))
                 .Select(item => new EnumValueResponse() { Name = item.Key, Value = item.Value })
                 .ToList()
        };
    }
}
