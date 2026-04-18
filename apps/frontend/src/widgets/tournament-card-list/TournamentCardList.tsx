import { TournamentCard } from '@/entities/tournament/ui/TournamentCard';
import type { TournamentResponse } from '@/shared/types/api';
import { EmptyState } from '@/shared/ui/EmptyState';

type TournamentCardListProps = {
  tournaments: TournamentResponse[];
  emptyTitle?: string;
  emptyDescription?: string;
};

export const TournamentCardList = ({
  tournaments,
  emptyTitle = 'Турниров пока нет',
  emptyDescription = 'Как только организаторы подготовят события, они появятся здесь.',
}: TournamentCardListProps) => {
  if (!tournaments.length) {
    return <EmptyState title={emptyTitle} description={emptyDescription} />;
  }

  return (
    <div className="cards-grid">
      {tournaments.map((tournament) => (
        <TournamentCard key={tournament.id} tournament={tournament} />
      ))}
    </div>
  );
};
