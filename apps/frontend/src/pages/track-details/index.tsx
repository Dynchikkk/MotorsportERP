import { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useAuth } from '@/app/providers/AuthProvider';
import { UserBadge } from '@/entities/user/ui/UserBadge';
import { TrackForm } from '@/features/tracks/create-track/TrackForm';
import { VoteTrackAction } from '@/features/tracks/vote-track/VoteTrackAction';
import { tracksApi } from '@/shared/api/tracks';
import { resolveMediaUrl } from '@/shared/lib/media';
import { useAsyncData } from '@/shared/lib/hooks/useAsyncData';
import { isModeratorOrAbove } from '@/shared/lib/roles';
import { TrackStatus } from '@/shared/types/api';
import { trackStatusLabels } from '@/shared/lib/format';
import { Button } from '@/shared/ui/Button';
import { Card } from '@/shared/ui/Card';
import { Loader } from '@/shared/ui/Loader';
import { Notice } from '@/shared/ui/Notice';
import { PageHeader } from '@/shared/ui/PageHeader';
import { PhotoUploader } from '@/shared/ui/PhotoUploader';
import { Pill } from '@/shared/ui/Pill';
import { TournamentCardList } from '@/widgets/tournament-card-list/TournamentCardList';

export const TrackDetailsPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { currentUser } = useAuth();
  const [isEditing, setIsEditing] = useState(false);

  const data = useAsyncData(
    async () => {
      if (!id) {
        throw new Error('Не удалось определить трассу');
      }

      const [track, reference] = await Promise.all([tracksApi.getById(id), tracksApi.getReferenceData()]);
      return { track, reference };
    },
    [id],
  );

  if (data.isLoading && !data.data) {
    return <Loader label="Загружаем трассу" fullHeight />;
  }

  if (data.error || !data.data) {
    return <Notice tone="error" message={data.error ?? 'Не удалось загрузить трассу'} />;
  }

  const { track, reference } = data.data;
  const isOwner = Boolean(currentUser && track.createdBy?.id === currentUser.id);
  const canModerate = isModeratorOrAbove(currentUser?.roles);
  const canEdit = isOwner || canModerate;
  const canVote = Boolean(currentUser && !isOwner && track.status === TrackStatus.Unofficial);

  const handleDelete = async () => {
    if (!id || !window.confirm('Удалить трассу?')) {
      return;
    }

    await tracksApi.remove(id);
    navigate('/tracks');
  };

  return (
    <div className="page">
      <PageHeader
        eyebrow="Детали трассы"
        title={track.name}
        description={`${track.location}. Здесь можно посмотреть, сколько голосов уже набрано, кто добавил трассу и какие турниры на ней планируются.`}
        actions={
          <Pill
            tone={
              track.status === TrackStatus.Unofficial
                ? 'warning'
                : track.status === TrackStatus.Confirmed
                  ? 'success'
                  : 'accent'
            }
          >
            {trackStatusLabels[track.status]}
          </Pill>
        }
      />

      <Card className="page__section">
        <div className="split">
          <div className="stack">
            <div className="cluster">
              <Pill tone="muted">{track.voteCount} голосов</Pill>
              <Pill tone="muted">Порог: {track.confirmationThreshold}</Pill>
            </div>
            {track.createdBy ? <UserBadge user={track.createdBy} /> : null}
            {track.photos.length ? (
              <div className="photo-strip">
                {track.photos.map((photo) => (
                  <img key={photo.id} src={resolveMediaUrl(photo.url) ?? ''} alt={track.name} />
                ))}
              </div>
            ) : null}
          </div>
          <div className="stack">
            {canVote ? <VoteTrackAction trackId={track.id} onVoted={data.reload} /> : null}
            {canEdit ? (
              <div className="inline-actions">
                <Button variant="secondary" onClick={() => setIsEditing((value) => !value)}>
                  {isEditing ? 'Скрыть форму' : 'Редактировать'}
                </Button>
                <PhotoUploader
                  label="Добавить фото"
                  onUploaded={async (file) => {
                    await tracksApi.addPhoto(track.id, file.id);
                    await data.reload();
                  }}
                />
                <Button variant="danger" onClick={() => void handleDelete()}>
                  Удалить
                </Button>
              </div>
            ) : null}
            {canModerate && track.status === TrackStatus.Unofficial ? (
              <Button
                variant="primary"
                onClick={() =>
                  void tracksApi.confirm(track.id).then(async () => {
                    await data.reload();
                  })
                }
              >
                Подтвердить вручную
              </Button>
            ) : null}
            {canModerate && track.status === TrackStatus.Confirmed ? (
              <Button
                variant="primary"
                onClick={() =>
                  void tracksApi.makeOfficial(track.id).then(async () => {
                    await data.reload();
                  })
                }
              >
                Сделать официальной
              </Button>
            ) : null}
          </div>
        </div>
      </Card>

      {isEditing && canEdit ? (
        <TrackForm
          initialValue={track}
          statusOptions={reference.trackStatuses}
          allowStatusSelection={canModerate}
          onSuccess={async () => {
            await data.reload();
            setIsEditing(false);
          }}
          onCancel={() => setIsEditing(false)}
        />
      ) : null}

      <Card className="page__section">
        <h2>Ближайшие турниры</h2>
        <TournamentCardList
          tournaments={track.upcomingTournaments}
          emptyTitle="Турниры на этой трассе пока не запланированы"
          emptyDescription="Как только организатор выберет эту площадку для нового события, карточка турнира появится здесь."
        />
      </Card>

      <Card className="page__section">
        <h2>Прошедшие турниры</h2>
        <TournamentCardList
          tournaments={track.pastTournaments}
          emptyTitle="На этой трассе ещё нет завершённых турниров"
          emptyDescription="После первого проведённого события здесь начнёт собираться история результатов."
        />
      </Card>
    </div>
  );
};
