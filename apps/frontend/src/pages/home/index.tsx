import { tournamentsApi } from '@/shared/api/tournaments';
import { tracksApi } from '@/shared/api/tracks';
import { useAsyncData } from '@/shared/lib/hooks/useAsyncData';
import { Loader } from '@/shared/ui/Loader';
import { Notice } from '@/shared/ui/Notice';
import { PageHeader } from '@/shared/ui/PageHeader';
import { TrackCardList } from '@/widgets/track-card-list/TrackCardList';
import { TournamentCardList } from '@/widgets/tournament-card-list/TournamentCardList';

export const HomePage = () => {
  const overview = useAsyncData(
    async () => {
      const [tournaments, tracks, tournamentReference, trackReference] = await Promise.all([
        tournamentsApi.getAll({}, 0, 3),
        tracksApi.getAll({}, 0, 3),
        tournamentsApi.getReferenceData(),
        tracksApi.getReferenceData(),
      ]);

      return {
        tournaments,
        tracks,
        tournamentReference,
        trackReference,
      };
    },
    [],
  );

  if (overview.isLoading && !overview.data) {
    return <Loader label="Собираем обзор платформы" fullHeight />;
  }

  if (overview.error || !overview.data) {
    return <Notice tone="error" message={overview.error ?? 'Не удалось загрузить обзор'} />;
  }

  const { tournaments, tracks, tournamentReference, trackReference } = overview.data;

  return (
    <div className="page">
      <section className="hero">
        <PageHeader
          eyebrow="Motorsport ERP"
          title="Турниры, трассы, заявки и история пилотов в одной системе"
          description="Здесь можно собирать турнир на подтверждённой трассе, вести заявки участников, хранить гаражи пилотов и публиковать результаты без разрыва между фронтом и серверной логикой."
        />
        <div className="stats">
          <div className="stat">
            <div className="stat__label">Гонок для добавления трассы</div>
            <div className="stat__value">{trackReference.minRacesToCreateTrack}</div>
          </div>
          <div className="stat">
            <div className="stat__label">Голосов для подтверждения</div>
            <div className="stat__value">{trackReference.defaultTrackConfirmationThreshold}</div>
          </div>
          <div className="stat">
            <div className="stat__label">Гонок для организатора</div>
            <div className="stat__value">{tournamentReference.minRacesToBecomeOrganizer}</div>
          </div>
          <div className="stat">
            <div className="stat__label">Ключевых сущностей</div>
            <div className="stat__value">4</div>
          </div>
        </div>
      </section>

      <section className="page__section">
        <PageHeader
          eyebrow="Ближайшие события"
          title="Турниры"
          description="В карточках сразу видно статус турнира, даты, трассу, допустимый класс автомобиля, количество заявок и сколько участников уже подтверждено."
        />
        <TournamentCardList tournaments={tournaments.items} />
      </section>

      <section className="page__section">
        <PageHeader
          eyebrow="Каталог трасс"
          title="Трассы сообщества"
          description="Пользовательские трассы сначала попадают в неофициальные, затем набирают голоса сообщества и после этого становятся доступными для турниров."
        />
        <TrackCardList tracks={tracks.items} />
      </section>
    </div>
  );
};
