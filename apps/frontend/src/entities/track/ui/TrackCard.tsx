import { Link } from 'react-router-dom';
import { PATHS } from '@/shared/config/paths';
import { trackStatusLabels } from '@/shared/lib/format';
import { resolveMediaUrl } from '@/shared/lib/media';
import { TrackStatus, type TrackResponse } from '@/shared/types/api';
import { Card } from '@/shared/ui/Card';
import { Pill } from '@/shared/ui/Pill';

type TrackCardProps = {
  track: TrackResponse;
};

const toneByStatus: Record<TrackStatus, 'warning' | 'success' | 'accent'> = {
  [TrackStatus.Unofficial]: 'warning',
  [TrackStatus.Confirmed]: 'success',
  [TrackStatus.Official]: 'accent',
};

export const TrackCard = ({ track }: TrackCardProps) => {
  return (
    <Card className="entity-card">
      <div className="entity-card__top">
        <div>
          <Link to={PATHS.TRACKS.DETAILS(track.id)}>
            <h3 className="entity-card__title">{track.name}</h3>
          </Link>
          <p className="muted">{track.location}</p>
        </div>
        <Pill tone={toneByStatus[track.status]}>{trackStatusLabels[track.status]}</Pill>
      </div>
      {track.photos[0] ? (
        <img className="photo-frame" src={resolveMediaUrl(track.photos[0].url) ?? ''} alt={track.name} />
      ) : null}
      <div className="cluster">
        <Pill tone="muted">{track.voteCount} голосов сообщества</Pill>
      </div>
    </Card>
  );
};
