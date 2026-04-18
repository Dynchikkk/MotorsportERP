import { useAuth } from '@/app/providers/AuthProvider';
import { UserBadge } from '@/entities/user/ui/UserBadge';
import { UpdateProfileForm } from '@/features/profile/update-profile/UpdateProfileForm';
import { getRoleNames, roleLabels, tournamentApplicationStatusLabels, tournamentStatusLabels } from '@/shared/lib/format';
import { Card } from '@/shared/ui/Card';
import { EmptyState } from '@/shared/ui/EmptyState';
import { PageHeader } from '@/shared/ui/PageHeader';
import { Pill } from '@/shared/ui/Pill';
import { TournamentCardList } from '@/widgets/tournament-card-list/TournamentCardList';

export const ProfilePage = () => {
  const { currentUser, refreshCurrentUser } = useAuth();

  if (!currentUser) {
    return <EmptyState title="Профиль недоступен" description="Сначала выполните вход в систему." />;
  }

  const roles = getRoleNames(currentUser.roles);

  return (
    <div className="page">
      <PageHeader
        eyebrow="Личный кабинет"
        title={currentUser.nickname}
        description="Здесь собраны ваши роли, заявки, организуемые турниры и публичные данные профиля."
      />

      <Card className="page__section">
        <div className="split">
          <div className="stack">
            <UserBadge user={currentUser} showRoles />
            <p className="muted">
              {currentUser.bio || 'Добавьте короткое описание, чтобы другим пилотам и организаторам было проще вас узнать.'}
            </p>
            <div className="stats">
              <div className="stat">
                <div className="stat__label">Гонок</div>
                <div className="stat__value">{currentUser.raceCount}</div>
              </div>
              <div className="stat">
                <div className="stat__label">Машин</div>
                <div className="stat__value">{currentUser.carsCount}</div>
              </div>
              <div className="stat">
                <div className="stat__label">Участий</div>
                <div className="stat__value">{currentUser.tournamentsCount}</div>
              </div>
            </div>
          </div>
          <div className="stack">
            <Card soft>
              <h3>Роли</h3>
              <div className="cluster">
                {roles.map((role) => (
                  <Pill key={role} tone="accent">
                    {roleLabels[role]}
                  </Pill>
                ))}
              </div>
            </Card>
            <Card soft>
              <h3>Статус</h3>
              <Pill tone={currentUser.isBlocked ? 'danger' : 'success'}>
                {currentUser.isBlocked ? 'Аккаунт заблокирован' : 'Аккаунт активен'}
              </Pill>
            </Card>
          </div>
        </div>
      </Card>

      <UpdateProfileForm profile={currentUser} onUpdated={async () => { await refreshCurrentUser(); }} />

      <Card className="page__section">
        <h2>Мои заявки</h2>
        {currentUser.applications.length ? (
          <div className="list">
            {currentUser.applications.map((entry) => (
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
          <EmptyState title="Заявок пока нет" description="Когда вы подадите первую заявку на турнир, она появится здесь." />
        )}
      </Card>

      <Card className="page__section">
        <h2>Организуемые турниры</h2>
        <TournamentCardList
          tournaments={currentUser.organizedTournaments}
          emptyTitle="Организуемых турниров пока нет"
          emptyDescription="После создания или получения доступа к турниру он появится здесь."
        />
      </Card>
    </div>
  );
};
