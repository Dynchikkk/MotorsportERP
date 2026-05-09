using MotorsportErp.Domain.Tracks;

namespace MotorsportErp.Application.Features.Tracks.Contracts;

public sealed record TrackVotingInfo(
    TrackStatus Status,
    Guid CreatedById,
    int ConfirmationThreshold);
