import { Link } from 'react-router-dom';
import { PATHS } from '@/shared/config/paths';
import { carClassLabels, formatDateRange, tournamentStatusLabels } from '@/shared/lib/format';
import { resolveMediaUrl } from '@/shared/lib/media';
import { TournamentStatus, type TournamentResponse } from '@/shared/types/api';
import { Card } from '@/shared/ui/Card';
import { Pill } from '@/shared/ui/Pill';

type TournamentCardProps = {
  tournament: TournamentResponse;
};

const toneByStatus: Record<TournamentStatus, 'accent' | 'success' | 'warning' | 'danger' | 'muted'> = {
  [TournamentStatus.RegistrationOpen]: 'accent',
  [TournamentStatus.Confirmed]: 'success',
  [TournamentStatus.Active]: 'warning',
  [TournamentStatus.Finished]: 'muted',
  [TournamentStatus.Cancelled]: 'danger',
};

export const TournamentCard = ({ tournament }: TournamentCardProps) => {
  return (
    <Card className="entity-card">
      <div className="entity-card__top">
        <div>
          <Link to={PATHS.TOURNAMENTS.DETAILS(tournament.id)}>
            <h3 className="entity-card__title">{tournament.name}</h3>
          </Link>
          <p className="muted">
            {tournament.trackName} • {formatDateRange(tournament.startDate, tournament.endDate)}
          </p>
        </div>
        <Pill tone={toneByStatus[tournament.status]}>{tournamentStatusLabels[tournament.status]}</Pill>
      </div>
      <div className="cluster">
        <Pill tone="muted">{carClassLabels[tournament.allowedCarClass]}</Pill>
        <Pill tone="muted">{tournament.participantsCount} подтверждено</Pill>
        <Pill tone="muted">{tournament.applicationsCount} заявок</Pill>
      </div>
      {tournament.photos[0] ? (
        <img className="photo-frame" src={resolveMediaUrl(tournament.photos[0].url) ?? ''} alt={tournament.name} />
      ) : null}
    </Card>
  );
};
