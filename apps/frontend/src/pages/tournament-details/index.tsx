import { useEffect, useMemo, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useAuth } from '@/app/providers/AuthProvider';
import { UserBadge } from '@/entities/user/ui/UserBadge';
import { TournamentApplyForm } from '@/features/tournaments/apply-to-tournament/TournamentApplyForm';
import { AddResultForm } from '@/features/tournaments/add-result/AddResultForm';
import { TournamentForm } from '@/features/tournaments/create-tournament/TournamentForm';
import { ApplicationActions } from '@/features/tournaments/manage-applications/ApplicationActions';
import { carsApi } from '@/shared/api/cars';
import { tournamentsApi } from '@/shared/api/tournaments';
import { tracksApi } from '@/shared/api/tracks';
import { usersApi } from '@/shared/api/users';
import { useAsyncData } from '@/shared/lib/hooks/useAsyncData';
import { isModeratorOrAbove } from '@/shared/lib/roles';
import {
  formatDateRange,
  formatLapTime,
  tournamentApplicationStatusLabels,
  tournamentStatusLabels,
} from '@/shared/lib/format';
import { getErrorMessage } from '@/shared/lib/errors';
import { resolveMediaUrl } from '@/shared/lib/media';
import { TournamentStatus } from '@/shared/types/api';
import { Button } from '@/shared/ui/Button';
import { Card } from '@/shared/ui/Card';
import { EmptyState } from '@/shared/ui/EmptyState';
import { SelectField } from '@/shared/ui/Field';
import { Loader } from '@/shared/ui/Loader';
import { Notice } from '@/shared/ui/Notice';
import { PageHeader } from '@/shared/ui/PageHeader';
import { PhotoUploader } from '@/shared/ui/PhotoUploader';
import { Pill } from '@/shared/ui/Pill';

export const TournamentDetailsPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { currentUser, refreshCurrentUser } = useAuth();
  const [isEditing, setIsEditing] = useState(false);
  const [candidateUserId, setCandidateUserId] = useState('');
  const [organizerFeedback, setOrganizerFeedback] = useState<{
    type: 'success' | 'error';
    message: string;
  } | null>(null);

  const data = useAsyncData(
    async () => {
      if (!id) {
        throw new Error('Не удалось определить турнир');
      }

      const [tournament, carReference, tracks, users] = await Promise.all([
        tournamentsApi.getById(id),
        carsApi.getReferenceData(),
        tracksApi.getAll({}, 0, 100),
        usersApi.getUsers('', 0, 100),
      ]);

      return {
        tournament,
        carReference,
        tracks: tracks.items.filter((track) => track.status !== 0),
        users: users.items,
      };
    },
    [id],
  );

  const applications = useAsyncData(
    async () => {
      if (!id) {
        throw new Error('Не удалось определить турнир');
      }

      return tournamentsApi.getApplications(id);
    },
    [id, currentUser?.id],
    {
      immediate: false,
      initialData: null,
    },
  );

  const myCars = useAsyncData(
    async () => {
      const response = await carsApi.getMyCars();
      return response.items;
    },
    [currentUser?.id],
    {
      immediate: Boolean(currentUser),
      initialData: currentUser ? [] : null,
    },
  );

  const canModerate = isModeratorOrAbove(currentUser?.roles);
  const isOrganizer = Boolean(
    currentUser && data.data?.tournament.organizers.some((organizer) => organizer.id === currentUser.id),
  );
  const canManageTournament = isOrganizer || canModerate;
  const tournamentId = data.data?.tournament.id;
  const shouldLoadApplications = Boolean(tournamentId && currentUser && canManageTournament);
  const reloadApplications = applications.reload;

  useEffect(() => {
    if (!shouldLoadApplications) {
      return;
    }

    void reloadApplications();
    const intervalId = window.setInterval(() => {
      void reloadApplications();
    }, 30_000);

    return () => {
      window.clearInterval(intervalId);
    };
  }, [reloadApplications, shouldLoadApplications]);
  
  const resultUserIds = useMemo(() => {
    return data.data?.tournament.results.map((result) => result.user.id) ?? [];
  }, [data.data?.tournament.results]);
  
  if (data.isLoading && !data.data) {
    return <Loader label="Загружаем турнир" fullHeight />;
  }

  if (data.error || !data.data) {
    return <Notice tone="error" message={data.error ?? 'Не удалось загрузить турнир'} />;
  }

  const { tournament, carReference, tracks, users } = data.data;
  const myApplication = currentUser?.applications.find((application) => application.tournamentId === tournament.id);
  const canApply =
    Boolean(currentUser) &&
    tournament.status === TournamentStatus.RegistrationOpen &&
    !myApplication &&
    !currentUser?.isBlocked;
  const organizerOptions = users.filter((user) => !tournament.organizers.some((organizer) => organizer.id === user.id));

  const handleTournamentAction = async (action: () => Promise<void>) => {
    await action();
    await data.reload();
    await refreshCurrentUser();
  };

  const handleDelete = async () => {
    if (!id || !window.confirm('Удалить турнир?')) {
      return;
    }

    await tournamentsApi.remove(id);
    navigate('/tournaments');
  };

  const handleOrganizerAdd = async () => {
    if (!candidateUserId) {
      return;
    }

    try {
      await tournamentsApi.addOrganizer(tournament.id, candidateUserId);
      await data.reload();
      setCandidateUserId('');
      setOrganizerFeedback({
        type: 'success',
        message: 'Соорганизатор добавлен',
      });
    } catch (submitError) {
      setOrganizerFeedback({
        type: 'error',
        message: getErrorMessage(submitError, 'Не удалось назначить соорганизатора'),
      });
    }
  };


  return (
    <div className="page">
      <PageHeader
        eyebrow="Детали турнира"
        title={tournament.name}
        description={`${tournament.track?.name ?? 'Трасса не определена'} • ${formatDateRange(
          tournament.startDate,
          tournament.endDate,
        )}. На странице собраны регламент, заявки, подтверждённые участники, организаторы и результаты заездов.`}
        actions={
          <Pill
            tone={
              tournament.status === TournamentStatus.RegistrationOpen
                ? 'accent'
                : tournament.status === TournamentStatus.Confirmed
                  ? 'success'
                  : tournament.status === TournamentStatus.Active
                    ? 'warning'
                    : tournament.status === TournamentStatus.Cancelled
                      ? 'danger'
                      : 'muted'
            }
          >
            {tournamentStatusLabels[tournament.status]}
          </Pill>
        }
      />

      <Card className="page__section">
        <div className="split">
          <div className="stack">
            <p>{tournament.description}</p>
            <div className="cluster">
              <Pill tone="muted">Участников: {tournament.participantsCount}/{tournament.requiredParticipants}</Pill>
              <Pill tone="muted">Заявок: {tournament.applicationsCount}</Pill>
              <Pill tone="muted">Минимальный стаж: {tournament.requiredRaceCount}</Pill>
            </div>
            {tournament.photos.length ? (
              <div className="photo-strip">
                {tournament.photos.map((photo) => (
                  <img key={photo.id} src={resolveMediaUrl(photo.url) ?? ''} alt={tournament.name} />
                ))}
              </div>
            ) : null}
          </div>
          <div className="stack">
            <Card soft>
              <h3>Организаторы</h3>
              <div className="list">
                {tournament.organizers.map((organizer) => (
                  <div key={organizer.id} className="list-item">
                    <UserBadge user={organizer} />
                  </div>
                ))}
              </div>
            </Card>
            {canManageTournament ? (
              <div className="inline-actions">
                <Button variant="secondary" onClick={() => setIsEditing((value) => !value)}>
                  {isEditing ? 'Скрыть форму' : 'Редактировать'}
                </Button>
                {tournament.status === TournamentStatus.Confirmed ? (
                  <Button variant="primary" onClick={() => void handleTournamentAction(() => tournamentsApi.start(tournament.id))}>
                    Запустить
                  </Button>
                ) : null}
                {tournament.status === TournamentStatus.Active ? (
                  <Button variant="primary" onClick={() => void handleTournamentAction(() => tournamentsApi.finish(tournament.id))}>
                    Завершить
                  </Button>
                ) : null}
                {tournament.status !== TournamentStatus.Finished && tournament.status !== TournamentStatus.Cancelled ? (
                  <Button variant="danger" onClick={() => void handleTournamentAction(() => tournamentsApi.cancel(tournament.id))}>
                    Отменить
                  </Button>
                ) : null}
                <PhotoUploader
                  label="Добавить фото"
                  onUploaded={async (file) => {
                    await tournamentsApi.addPhoto(tournament.id, file.id);
                    await data.reload();
                  }}
                />
                {canModerate ? (
                  <Button variant="danger" onClick={() => void handleDelete()}>
                    Удалить
                  </Button>
                ) : null}
              </div>
            ) : null}
          </div>
        </div>
      </Card>

      {isEditing && canManageTournament ? (
        <TournamentForm
          initialValue={tournament}
          tracks={tracks}
          carReference={carReference}
          onSuccess={async () => {
            await data.reload();
            setIsEditing(false);
          }}
          onCancel={() => setIsEditing(false)}
        />
      ) : null}

      {canApply && myCars.data ? (
        <TournamentApplyForm
          tournament={tournament}
          cars={myCars.data}
          onApplied={async () => {
            await refreshCurrentUser();
            await data.reload();
          }}
        />
      ) : null}

      {myApplication ? (
        <Card className="page__section">
          <h2>Моя заявка</h2>
          <div className="cluster">
            <Pill tone="accent">{tournamentApplicationStatusLabels[myApplication.applicationStatus]}</Pill>
          </div>
          {tournament.status === TournamentStatus.RegistrationOpen ? (
            <Button
              variant="ghost"
              onClick={() =>
                void tournamentsApi.cancelApplication(myApplication.applicationId).then(async () => {
                  await refreshCurrentUser();
                  await data.reload();
                })
              }
            >
              Отменить заявку
            </Button>
          ) : null}
        </Card>
      ) : null}

      {canManageTournament ? (
        <>
          <Card className="page__section">
            <h2>Соорганизаторы</h2>
            {organizerOptions.length ? (
              <div className="form__row">
                <SelectField
                  label="Добавить пользователя"
                  value={candidateUserId}
                  onChange={(event) => setCandidateUserId(event.target.value)}
                >
                  <option value="">Выберите пользователя</option>
                  {organizerOptions.map((user) => (
                    <option key={user.id} value={user.id}>
                      {user.nickname}
                    </option>
                  ))}
                </SelectField>
                <div className="stack">
                  <Button variant="secondary" onClick={() => void handleOrganizerAdd()} disabled={!candidateUserId}>
                    Назначить
                  </Button>
                </div>
              </div>
            ) : (
              <EmptyState title="Свободных кандидатов не осталось" description="Все пользователи из текущей выборки уже добавлены в команду организаторов." />
            )}
            {organizerFeedback ? <Notice tone={organizerFeedback.type} message={organizerFeedback.message} /> : null}
          </Card>

          <Card className="page__section">
            <h2>Заявки участников</h2>
            {applications.isLoading && !applications.data ? <Loader label="Загружаем заявки" /> : null}
            {applications.error ? <Notice tone="error" message={applications.error} /> : null}
            {applications.data?.length ? (
              <div className="list">
                {applications.data.map((application) => (
                  <div key={application.id} className="list-item">
                    <div className="toolbar">
                      <UserBadge user={application.user} />
                      <Pill tone="muted">{tournamentApplicationStatusLabels[application.status]}</Pill>
                    </div>
                    <p className="muted" style={{ margin: '8px 0' }}>
                      Автомобиль: {application.car.brand} {application.car.model}
                    </p>
                    {application.status === 0 ? (
                      <ApplicationActions
                        applicationId={application.id}
                        onChanged={async () => {
                          await applications.reload();
                          await data.reload();
                          await refreshCurrentUser();
                        }}
                      />
                    ) : null}
                  </div>
                ))}
              </div>
            ) : (
              <EmptyState title="Новых заявок пока нет" description="Когда пилоты отправят заявки на участие, здесь можно будет подтверждать или отклонять их вручную." />
            )}
          </Card>
        </>
      ) : null}

      {tournament.status === TournamentStatus.Active && canManageTournament ? (
        <AddResultForm
          tournamentId={tournament.id}
          participants={tournament.participants}
          existingResultUserIds={resultUserIds}
          onAdded={data.reload}
        />
      ) : null}

      <Card className="page__section">
        <h2>Подтверждённые участники</h2>
        {tournament.participants.length ? (
          <div className="list">
            {tournament.participants.map((participant) => (
              <div key={participant.id} className="list-item">
                <div className="toolbar">
                  <UserBadge user={participant.user} />
                  <Pill tone="success">
                    {participant.car.brand} {participant.car.model}
                  </Pill>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <EmptyState title="Подтверждённых участников пока нет" description="Список начнёт наполняться после одобрения заявок организатором или модератором." />
        )}
      </Card>

      <Card className="page__section">
        <h2>Результаты</h2>
        {tournament.results.length ? (
          <div className="list">
            {tournament.results.map((result) => (
              <div key={result.id} className="list-item">
                <div className="toolbar">
                  <UserBadge user={result.user} />
                  <div className="cluster">
                    <Pill tone="accent">#{result.position}</Pill>
                    <Pill tone="muted">{formatLapTime(result.bestLapTime)}</Pill>
                  </div>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <EmptyState title="Результаты ещё не внесены" description="Когда турнир перейдёт в активную фазу, здесь можно будет фиксировать позиции и лучшее время круга." />
        )}
      </Card>
    </div>
  );
};
