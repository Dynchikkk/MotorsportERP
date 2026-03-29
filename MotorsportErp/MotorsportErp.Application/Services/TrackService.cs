using Microsoft.Extensions.Options;
using MotorsportErp.Application.Common.Settings;
using MotorsportErp.Application.DTO.Tracks;
using MotorsportErp.Application.Interfaces.Repositories;
using MotorsportErp.Application.Interfaces.Services;
using MotorsportErp.Application.Mappers;
using MotorsportErp.Domain.Tournaments;
using MotorsportErp.Domain.Tracks;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.Services;

public class TrackService : ITrackService
{
    private readonly ITrackRepository _trackRepository;
    private readonly IUserRepository _userRepository;
    private readonly TrackSettings _settings;

    public TrackService(
        ITrackRepository trackRepository,
        IUserRepository userRepository,
        IOptions<TrackSettings> options)
    {
        _trackRepository = trackRepository;
        _userRepository = userRepository;
        _settings = options.Value;
    }

    public async Task<List<TrackResponse>> GetAllAsync()
    {
        List<Track> tracks = await _trackRepository.GetAllAsync();
        return TrackMapper.ToResponseList(tracks).ToList();
    }

    public async Task<TrackDetailsResponse> GetByIdAsync(Guid id)
    {
        Track track = await _trackRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Track not found");
        return TrackMapper.ToDetails(track);
    }

    public async Task<Guid> CreateAsync(Guid userId, TrackCreateRequest request)
    {
        Domain.Users.User user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");

        bool isPrivileged = user.Roles.HasFlag(UserRole.Moderator) || user.Roles.HasFlag(UserRole.SuperAdmin);

        if (!isPrivileged && user.RaceCount < _settings.MinRacesToCreate)
        {
            throw new UnauthorizedAccessException("Not enough races to create track");
        }

        Track track = TrackMapper.ToEntity(request, userId);
        track.ConfirmationThreshold = _settings.DefaultConfirmationThreshold;

        if (isPrivileged && request.Status.HasValue)
        {
            track.Status = request.Status.Value;
        }

        await _trackRepository.AddAsync(track);

        return track.Id;
    }

    public async Task VoteAsync(Guid userId, Guid trackId, bool isPositive)
    {
        var track = await _trackRepository.GetByIdAsync(trackId) ?? throw new KeyNotFoundException("Track not found");

        if (track.Status != TrackStatus.Unofficial)
            throw new InvalidOperationException("Can only vote for unofficial tracks");

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
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        var track = await _trackRepository.GetByIdAsync(trackId)
            ?? throw new KeyNotFoundException("Track not found");

        bool isModerator = user.Roles.HasFlag(UserRole.Moderator) ||
                           user.Roles.HasFlag(UserRole.SuperAdmin);

        if (!isModerator)
        {
            throw new UnauthorizedAccessException("Only moderator can confirm track");
        }

        if (track.Status == TrackStatus.Confirmed)
        {
            throw new InvalidOperationException("Track already confirmed");
        }

        track.Status = TrackStatus.Confirmed;

        await _trackRepository.UpdateAsync(track);
    }

    public async Task UpdateAsync(Guid userId, Guid trackId, TrackUpdateRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        var track = await _trackRepository.GetByIdAsync(trackId)
            ?? throw new KeyNotFoundException("Track not found");

        bool isOwner = track.CreatedById == userId;
        bool isModerator = user.Roles.HasFlag(UserRole.Moderator) ||
                           user.Roles.HasFlag(UserRole.SuperAdmin);

        if (!isOwner && !isModerator)
        {
            throw new UnauthorizedAccessException("You cannot edit this track");
        }

        track.Name = request.Name;
        track.Location = request.Location;
        track.LayoutImageUrl = request.LayoutImageUrl;

        await _trackRepository.UpdateAsync(track);
    }

    public async Task DeleteAsync(Guid userId, Guid trackId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        var track = await _trackRepository.GetByIdAsync(trackId)
            ?? throw new KeyNotFoundException("Track not found");

        bool isOwner = track.CreatedById == userId;
        bool isModerator = user.Roles.HasFlag(UserRole.Moderator) ||
                           user.Roles.HasFlag(UserRole.SuperAdmin);

        if (!isOwner && !isModerator)
        {
            throw new UnauthorizedAccessException("You cannot delete this track");
        }

        if (track.Tournaments.Any(t => t.Status != TournamentStatus.Finished))
        {
            throw new InvalidOperationException("Cannot delete track used in active tournaments");
        }

        await _trackRepository.DeleteAsync(track);
    }

    public async Task MakeOfficialAsync(Guid userId, Guid trackId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        var track = await _trackRepository.GetByIdAsync(trackId)
            ?? throw new KeyNotFoundException("Track not found");

        bool isModerator = user.Roles.HasFlag(UserRole.Moderator) ||
                           user.Roles.HasFlag(UserRole.SuperAdmin);

        if (!isModerator)
        {
            throw new UnauthorizedAccessException("Only moderator can make track official");
        }

        if (track.Status != TrackStatus.Confirmed)
        {
            throw new InvalidOperationException("Track must be confirmed first");
        }

        track.Status = TrackStatus.Official;

        await _trackRepository.UpdateAsync(track);
    }
}