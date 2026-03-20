using Microsoft.Extensions.Options;
using MotorsportErp.Application.Common.Settings;
using MotorsportErp.Application.DTO.Tracks;
using MotorsportErp.Application.Interfaces.Repositories;
using MotorsportErp.Application.Interfaces.Services;
using MotorsportErp.Application.Mappers;
using MotorsportErp.Domain.Tracks;

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

        if (user.RaceCount < _settings.MinRacesToCreate)
        {
            throw new UnauthorizedAccessException("Not enough races to create track");
        }

        Track track = TrackMapper.ToEntity(request, userId);
        track.ConfirmationThreshold = _settings.DefaultConfirmationThreshold;
        await _trackRepository.AddAsync(track);

        return track.Id;
    }

    public async Task VoteAsync(Guid userId, TrackVoteRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        var track = await _trackRepository.GetByIdAsync(request.TrackId) ?? throw new KeyNotFoundException("Track not found");
        
        if (track.Status != TrackStatus.Unofficial)
        {
            throw new InvalidOperationException("Voting is closed for this track");
        }

        bool alreadyVoted = await _trackRepository.HasUserVotedAsync(track.Id, user.Id);
        if (alreadyVoted)
        {
            throw new InvalidOperationException("User already voted for this track");
        }

        TrackVote vote = new()
        {
            Id = Guid.NewGuid(),
            TrackId = track.Id,
            UserId = userId
        };

        await _trackRepository.AddVoteAsync(vote);

        if (track.VoteCount + 1 >= track.ConfirmationThreshold)
        {
            track.Status = TrackStatus.Confirmed;
            await _trackRepository.UpdateAsync(track);
        }
    }
}