using Microsoft.Extensions.Options;
using MotorsportErp.Application.Common.Contracts;
using MotorsportErp.Application.Common.Exceptions;
using MotorsportErp.Application.Common.Extensions;
using MotorsportErp.Application.Common.Interfaces.Repositories;
using MotorsportErp.Application.Features.Tracks.Contracts;
using MotorsportErp.Application.Features.Tracks.Interfaces;
using MotorsportErp.Application.Features.Tracks.Mappers;
using MotorsportErp.Application.Features.Tracks.Settings;
using MotorsportErp.Domain.Tournaments;
using MotorsportErp.Domain.Tracks;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.Features.Tracks.Services;

public class TrackService : ITrackService
{
    private readonly ITrackRepository _trackRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFileRepository _fileRepository;

    private readonly TrackSettings _settings;

    public TrackService(
        ITrackRepository trackRepository,
        IUserRepository userRepository,
        IOptions<TrackSettings> options,
        IFileRepository fileRepository)
    {
        _trackRepository = trackRepository;
        _userRepository = userRepository;
        _settings = options.Value;
        _fileRepository = fileRepository;
    }

    public async Task<PagedResponse<TrackResponse>> GetAllAsync(TrackListQuery query, int page = 0, int pageSize = 20)
    {
        query ??= new TrackListQuery();
        var (tracks, totalCount) = await _trackRepository.GetFilteredPagedAsync(query, page, pageSize);

        return new PagedResponse<TrackResponse>
        {
            Items = tracks.Select(TrackMapper.ToResponse).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<TrackDetailsResponse> GetByIdAsync(Guid id)
    {
        Track track = await _trackRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Track not found");
        return TrackMapper.ToDetails(track);
    }

    public async Task<Guid> CreateAsync(Guid userId, TrackCreateRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        bool isModerator = user.Roles.HasFlag(UserRole.Moderator) || user.Roles.HasFlag(UserRole.SuperAdmin);
        if (!isModerator && user.RaceCount < _settings.MinRacesToCreate)
        {
            throw new ForbiddenException("Not enough races to create track");
        }

        Track track = TrackMapper.ToEntity(request, userId);
        track.Status = TrackStatus.Unofficial;
        track.ConfirmationThreshold = _settings.DefaultConfirmationThreshold;

        if (isModerator && request.Status.HasValue)
        {
            track.Status = request.Status.Value;
        }

        await _trackRepository.AddAsync(track);

        return track.Id;
    }

    public async Task VoteAsync(Guid userId, Guid trackId, bool isPositive)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        if (user.IsBlocked)
        {
            throw new UnauthorizedAccessException("User is blocked");
        }

        var track = await _trackRepository.GetByIdAsync(trackId) ?? throw new KeyNotFoundException("Track not found");
        if (track.Status != TrackStatus.Unofficial)
        {
            throw new InvalidOperationException("Can only vote for unofficial tracks");
        }

        if (track.CreatedById == userId)
        {
            throw new InvalidOperationException("Cannot vote for your own track");
        }

        var existingVote = track.Votes.FirstOrDefault(v => v.UserId == userId);
        if (existingVote != null)
        {
            existingVote.IsPositive = isPositive;
        }
        else
        {
            track.Votes.Add(new TrackVote
            {
                Id = Guid.NewGuid(),
                TrackId = trackId,
                UserId = userId,
                IsPositive = isPositive
            });
        }

        var positiveVotesCount = track.Votes.Count(v => v.IsPositive);
        if (positiveVotesCount >= track.ConfirmationThreshold)
        {
            track.Status = TrackStatus.Confirmed;
        }

        await _trackRepository.UpdateAsync(track);
    }

    public async Task ConfirmAsync(Guid userId, Guid trackId)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        bool isModerator = user.Roles.HasFlag(UserRole.Moderator) || user.Roles.HasFlag(UserRole.SuperAdmin);
        if (!isModerator)
        {
            throw new ForbiddenException("Only moderator can confirm track");
        }

        var track = await _trackRepository.GetByIdAsync(trackId) ?? throw new KeyNotFoundException("Track not found");
        if (track.Status == TrackStatus.Confirmed)
        {
            throw new InvalidOperationException("Track already confirmed");
        }

        track.Status = TrackStatus.Confirmed;

        await _trackRepository.UpdateAsync(track);
    }

    public async Task UpdateAsync(Guid userId, Guid trackId, TrackUpdateRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        if (user.IsBlocked)
        {
            throw new UnauthorizedAccessException("User is blocked");
        }

        var track = await _trackRepository.GetByIdAsync(trackId) ?? throw new KeyNotFoundException("Track not found");

        bool isOwner = track.CreatedById == userId;
        bool isModerator = user.Roles.HasFlag(UserRole.Moderator) || user.Roles.HasFlag(UserRole.SuperAdmin);
        if (!isOwner && !isModerator)
        {
            throw new ForbiddenException("You cannot edit this track");
        }

        track.Name = request.Name;
        track.Location = request.Location;

        await _trackRepository.UpdateAsync(track);
    }

    public async Task DeleteAsync(Guid userId, Guid trackId)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        var track = await _trackRepository.GetByIdAsync(trackId) ?? throw new KeyNotFoundException("Track not found");

        bool isOwner = track.CreatedById == userId;
        bool isModerator = user.Roles.HasFlag(UserRole.Moderator) || user.Roles.HasFlag(UserRole.SuperAdmin);
        if (!isOwner && !isModerator)
        {
            throw new ForbiddenException("You cannot delete this track");
        }

        if (track.Tournaments.Any(t => t.Status != TournamentStatus.Finished))
        {
            throw new InvalidOperationException("Cannot delete track used in active tournaments");
        }

        await _trackRepository.DeleteAsync(track);
    }

    public async Task MakeOfficialAsync(Guid userId, Guid trackId)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        bool isModerator = user.Roles.HasFlag(UserRole.Moderator) || user.Roles.HasFlag(UserRole.SuperAdmin);
        if (!isModerator)
        {
            throw new ForbiddenException("Only moderator can make track official");
        }

        var track = await _trackRepository.GetByIdAsync(trackId) ?? throw new KeyNotFoundException("Track not found");
        if (track.Status != TrackStatus.Confirmed)
        {
            throw new InvalidOperationException("Track must be confirmed first");
        }

        track.Status = TrackStatus.Official;

        await _trackRepository.UpdateAsync(track);
    }

    public async Task AddPhotoAsync(Guid userId, Guid targetEntityId, Guid photoId)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        if (user.IsBlocked)
        {
            throw new UnauthorizedAccessException("User is blocked");
        }

        var track = await _trackRepository.GetByIdAsync(targetEntityId) ?? throw new KeyNotFoundException("Track not found");

        bool isOwner = track.CreatedById == userId;
        bool isModerator = user.Roles.HasFlag(UserRole.Moderator) || user.Roles.HasFlag(UserRole.SuperAdmin);
        if (!isOwner && !isModerator)
        {
            throw new ForbiddenException("You cannot edit this track");
        }

        var photo = await _fileRepository.GetByIdAsync(photoId) ?? throw new KeyNotFoundException("Photo not found");
        if (photo.UploadedById != userId)
        {
            throw new ForbiddenException("Only owner can use self photos");
        }

        track.Photos.Add(photo);
        await _trackRepository.UpdateAsync(track);
    }

    public async Task RemovePhotoAsync(Guid userId, Guid targetEntityId, Guid photoId)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        if (user.IsBlocked)
        {
            throw new UnauthorizedAccessException("User is blocked");
        }

        var track = await _trackRepository.GetByIdAsync(targetEntityId) ?? throw new KeyNotFoundException("Track not found");

        bool isOwner = track.CreatedById == userId;
        bool isModerator = user.Roles.HasFlag(UserRole.Moderator) || user.Roles.HasFlag(UserRole.SuperAdmin);
        if (!isOwner && !isModerator)
        {
            throw new ForbiddenException("You cannot edit this track");
        }

        var photo = await _fileRepository.GetByIdAsync(photoId);
        if (photo != null)
        {
            _ = track.Photos.Remove(photo);
            await _trackRepository.UpdateAsync(track);
        }
    }

    public TrackReferenceDataResponse GetReferenceData()
    {
        return new TrackReferenceDataResponse()
        {
            MinRacesToCreateTrack = _settings.MinRacesToCreate,
            DefaultTrackConfirmationThreshold = _settings.DefaultConfirmationThreshold,
            TrackStatuses = EnumExtensions.GetEnumValues<TrackStatus>()
                 .Select(item => new EnumValueResponse() { Name = item.Key, Value = item.Value })
                 .ToList()
        };
    }
}
