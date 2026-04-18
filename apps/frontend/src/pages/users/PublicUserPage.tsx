import { UserBadge } from '@/entities/user/ui/UserBadge';
import { useAsyncData } from '@/shared/lib/hooks/useAsyncData';
import { usersApi } from '@/shared/api/users';
import { tournamentApplicationStatusLabels, tournamentStatusLabels } from '@/shared/lib/format';
import { Card } from '@/shared/ui/Card';
import { EmptyState } from '@/shared/ui/EmptyState';
import { Loader } from '@/shared/ui/Loader';
import { Notice } from '@/shared/ui/Notice';
import { PageHeader } from '@/shared/ui/PageHeader';
import { Pill } from '@/shared/ui/Pill';
import { useParams } from 'react-router-dom';

export const PublicUserPage = () => {
  const { id } = useParams();

  const profile = useAsyncData(
    async () => {
      if (!id) {
        throw new Error('Не удалось определить пользователя');
      }

      return usersApi.getPublicProfile(id);
    },
    [id],
  );

  if (profile.isLoading && !profile.data) {
    return <Loader label="Загружаем публичный профиль" fullHeight />;
  }

  if (profile.error || !profile.data) {
    return <Notice tone="error" message={profile.error ?? 'Не удалось загрузить профиль'} />;
  }

  return (
    <div className="page">
      <PageHeader
        eyebrow="Публичный профиль"
        title={profile.data.nickname}
        description="Открытая карточка гонщика: био, машины и история участия в турнирах."
      />

      <Card className="page__section">
        <UserBadge user={profile.data} />
        <p className="muted">{profile.data.bio || 'Пилот пока не заполнил описание профиля.'}</p>
        <div className="stats">
          <div className="stat">
            <div className="stat__label">Гонок</div>
            <div className="stat__value">{profile.data.raceCount}</div>
          </div>
          <div className="stat">
            <div className="stat__label">Машин</div>
            <div className="stat__value">{profile.data.carsCount}</div>
          </div>
          <div className="stat">
            <div className="stat__label">Участий</div>
            <div className="stat__value">{profile.data.tournamentsCount}</div>
          </div>
        </div>
      </Card>

      <Card className="page__section">
        <h2>Гараж</h2>
        {profile.data.cars.length ? (
          <div className="list">
            {profile.data.cars.map((car) => (
              <div key={car.id} className="list-item">
                <strong>
                  {car.brand} {car.model}
                </strong>
                <p className="muted" style={{ margin: '6px 0' }}>
                  {car.year} • {car.description || 'Без описания'}
                </p>
              </div>
            ))}
          </div>
        ) : (
          <EmptyState title="Гараж пуст" description="Пользователь пока не добавил автомобили." />
        )}
      </Card>

      <Card className="page__section">
        <h2>Текущие участия</h2>
        {profile.data.currentTournaments.length ? (
          <div className="list">
            {profile.data.currentTournaments.map((entry) => (
              <div key={entry.applicationId} className="list-item">
                <strong>{entry.tournamentName}</strong>
                <p className="muted" style={{ margin: '6px 0' }}>
                  {entry.trackName} • {entry.carName}
                </p>
                <div className="cluster">
                  <Pill tone="muted">{tournamentStatusLabels[entry.tournamentStatus]}</Pill>
                  <Pill tone="accent">{tournamentApplicationStatusLabels[entry.applicationStatus]}</Pill>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <EmptyState title="Текущих участий нет" description="Пилот пока не участвует в активных турнирах." />
        )}
      </Card>

      <Card className="page__section">
        <h2>История турниров</h2>
        {profile.data.tournamentHistory.length ? (
          <div className="list">
            {profile.data.tournamentHistory.map((entry) => (
              <div key={entry.applicationId} className="list-item">
                <strong>{entry.tournamentName}</strong>
                <p className="muted" style={{ margin: '6px 0' }}>
                  {entry.trackName} • {entry.carName}
                </p>
                <div className="cluster">
                  <Pill tone="muted">{tournamentStatusLabels[entry.tournamentStatus]}</Pill>
                  <Pill tone="accent">{tournamentApplicationStatusLabels[entry.applicationStatus]}</Pill>
                  {entry.position ? <Pill tone="success">#{entry.position}</Pill> : null}
                </div>
              </div>
            ))}
          </div>
        ) : (
          <EmptyState title="История пока пуста" description="Завершённые участия пользователя будут появляться здесь." />
        )}
      </Card>
    </div>
  );
};
