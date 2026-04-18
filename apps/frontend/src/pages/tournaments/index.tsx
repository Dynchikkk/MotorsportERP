import { useDeferredValue, useState } from 'react';
import { useAuth } from '@/app/providers/AuthProvider';
import { TournamentForm } from '@/features/tournaments/create-tournament/TournamentForm';
import { carsApi } from '@/shared/api/cars';
import { tournamentsApi } from '@/shared/api/tournaments';
import { tracksApi } from '@/shared/api/tracks';
import { useAsyncData } from '@/shared/lib/hooks/useAsyncData';
import { isOrganizerOrAbove } from '@/shared/lib/roles';
import { tournamentStatusLabels } from '@/shared/lib/format';
import { Card } from '@/shared/ui/Card';
import { EmptyState } from '@/shared/ui/EmptyState';
import { InputField, SelectField } from '@/shared/ui/Field';
import { Loader } from '@/shared/ui/Loader';
import { Notice } from '@/shared/ui/Notice';
import { PageHeader } from '@/shared/ui/PageHeader';
import { TournamentCardList } from '@/widgets/tournament-card-list/TournamentCardList';
import type { TournamentStatus } from '@/shared/types/api';

export const TournamentsPage = () => {
  const { currentUser } = useAuth();
  const [search, setSearch] = useState('');
  const [status, setStatus] = useState('');
  const deferredSearch = useDeferredValue(search);

  const data = useAsyncData(
    async () => {
      const [tournaments, tournamentReference, carReference, tracks] = await Promise.all([
        tournamentsApi.getAll({
          search: deferredSearch || undefined,
          status: status ? Number(status) : undefined,
        }),
        tournamentsApi.getReferenceData(),
        carsApi.getReferenceData(),
        tracksApi.getAll({}, 0, 100),
      ]);

      return {
        tournaments,
        tournamentReference,
        carReference,
        tracks: tracks.items.filter((track) => track.status !== 0),
      };
    },
    [deferredSearch, status],
  );

  if (data.isLoading && !data.data) {
    return <Loader label="Загружаем турниры" fullHeight />;
  }

  if (data.error || !data.data) {
    return <Notice tone="error" message={data.error ?? 'Не удалось загрузить турниры'} />;
  }

  const canCreateTournament =
    Boolean(currentUser) &&
    (isOrganizerOrAbove(currentUser?.roles) ||
      (currentUser?.raceCount ?? 0) >= data.data.tournamentReference.minRacesToBecomeOrganizer);

  return (
    <div className="page">
      <PageHeader
        eyebrow="Турниры"
        title="События, регистрация и результаты"
        description="На этой странице можно найти нужный турнир по статусу, открыть регистрацию на новое событие и следить, сколько заявок уже принято и сколько участников подтверждено."
      />

      <Card className="page__section">
        <div className="form__row">
          <InputField
            label="Поиск"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Название турнира"
          />
          <SelectField label="Статус" value={status} onChange={(event) => setStatus(event.target.value)}>
            <option value="">Все статусы</option>
            {data.data.tournamentReference.tournamentStatuses.map((item) => (
              <option key={item.value} value={item.value}>
                {tournamentStatusLabels[item.value as TournamentStatus] ?? item.name}
              </option>
            ))}
          </SelectField>
        </div>
      </Card>

      {currentUser ? (
        data.data.tracks.length ? (
          canCreateTournament ? (
            <TournamentForm tracks={data.data.tracks} carReference={data.data.carReference} onSuccess={data.reload} />
          ) : (
            <Notice
              tone="error"
              message={`Сейчас создать турнир нельзя: нужно минимум ${data.data.tournamentReference.minRacesToBecomeOrganizer} завершённых гонок, если у вас ещё нет роли организатора.`}
            />
          )
        ) : (
          <Notice tone="error" message="Сначала нужна хотя бы одна подтверждённая или официальная трасса, которую можно выбрать местом проведения." />
        )
      ) : null}

      {data.data.tournaments.items.length ? (
        <TournamentCardList tournaments={data.data.tournaments.items} />
      ) : (
        <EmptyState title="Подходящих турниров пока нет" description="Попробуйте сменить фильтр или создайте новое событие, если вам уже доступна роль организатора." />
      )}
    </div>
  );
};
