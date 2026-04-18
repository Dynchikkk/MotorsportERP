import { TrackCard } from '@/entities/track/ui/TrackCard';
import type { TrackResponse } from '@/shared/types/api';
import { EmptyState } from '@/shared/ui/EmptyState';

type TrackCardListProps = {
  tracks: TrackResponse[];
  emptyTitle?: string;
  emptyDescription?: string;
};

export const TrackCardList = ({
  tracks,
  emptyTitle = 'Трассы пока не найдены',
  emptyDescription = 'Попробуйте изменить фильтры или вернитесь сюда позже.',
}: TrackCardListProps) => {
  if (!tracks.length) {
    return <EmptyState title={emptyTitle} description={emptyDescription} />;
  }

  return (
    <div className="cards-grid">
      {tracks.map((track) => (
        <TrackCard key={track.id} track={track} />
      ))}
    </div>
  );
};
