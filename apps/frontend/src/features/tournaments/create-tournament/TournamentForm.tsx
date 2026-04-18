import { useEffect, useState, type FormEvent } from 'react';
import { filesApi } from '@/shared/api/files';
import { tournamentsApi } from '@/shared/api/tournaments';
import { toDateTimeInputValue, toIsoDateTime } from '@/shared/lib/dates';
import { getErrorMessage } from '@/shared/lib/errors';
import { carClassLabels } from '@/shared/lib/format';
import { syncMediaFiles } from '@/shared/lib/media';
import type {
  CarClass,
  CarReferenceDataResponse,
  MediaFileResponse,
  TournamentDetailsResponse,
  TrackResponse,
} from '@/shared/types/api';
import { Button } from '@/shared/ui/Button';
import { Card } from '@/shared/ui/Card';
import { InputField, SelectField, TextAreaField } from '@/shared/ui/Field';
import { Notice } from '@/shared/ui/Notice';
import { PhotoGalleryEditor } from '@/shared/ui/PhotoGalleryEditor';

type TournamentFormProps = {
  tracks: TrackResponse[];
  carReference: CarReferenceDataResponse;
  initialValue?: TournamentDetailsResponse;
  onSuccess: () => Promise<void>;
  onCancel?: () => void;
};

export const TournamentForm = ({
  tracks,
  carReference,
  initialValue,
  onSuccess,
  onCancel,
}: TournamentFormProps) => {
  const [form, setForm] = useState({
    name: initialValue?.name ?? '',
    description: initialValue?.description ?? '',
    startDate: toDateTimeInputValue(initialValue?.startDate),
    endDate: toDateTimeInputValue(initialValue?.endDate),
    trackId: initialValue?.trackId ?? tracks[0]?.id ?? '',
    allowedCarClass: initialValue?.allowedCarClass ?? carReference.carClasses[0]?.value ?? 0,
    requiredRaceCount: initialValue?.requiredRaceCount ?? 0,
    requiredParticipants: initialValue?.requiredParticipants ?? 8,
  });
  const [photos, setPhotos] = useState<MediaFileResponse[]>(initialValue?.photos ?? []);
  const [feedback, setFeedback] = useState<{ type: 'success' | 'error'; message: string } | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    setForm({
      name: initialValue?.name ?? '',
      description: initialValue?.description ?? '',
      startDate: toDateTimeInputValue(initialValue?.startDate),
      endDate: toDateTimeInputValue(initialValue?.endDate),
      trackId: initialValue?.trackId ?? tracks[0]?.id ?? '',
      allowedCarClass: initialValue?.allowedCarClass ?? carReference.carClasses[0]?.value ?? 0,
      requiredRaceCount: initialValue?.requiredRaceCount ?? 0,
      requiredParticipants: initialValue?.requiredParticipants ?? 8,
    });
    setPhotos(initialValue?.photos ?? []);
  }, [carReference.carClasses, initialValue, tracks]);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setIsSubmitting(true);
    setFeedback(null);

    try {
      if (initialValue) {
        await tournamentsApi.update(initialValue.id, {
          description: form.description,
          startDate: toIsoDateTime(form.startDate),
          endDate: toIsoDateTime(form.endDate),
          requiredParticipants: form.requiredParticipants,
        });
        await syncMediaFiles({
          currentPhotos: initialValue.photos,
          nextPhotos: photos,
          addPhoto: (photoId) => tournamentsApi.addPhoto(initialValue.id, photoId),
          removePhoto: (photoId) => tournamentsApi.removePhoto(initialValue.id, photoId),
        });
      } else {
        const createdTournamentId = await tournamentsApi.create({
          name: form.name,
          description: form.description,
          startDate: toIsoDateTime(form.startDate),
          endDate: toIsoDateTime(form.endDate),
          trackId: form.trackId,
          allowedCarClass: form.allowedCarClass,
          requiredRaceCount: form.requiredRaceCount,
          requiredParticipants: form.requiredParticipants,
        });
        await Promise.all(photos.map((photo) => tournamentsApi.addPhoto(createdTournamentId, photo.id)));
      }

      await onSuccess();
      if (!initialValue) {
        setPhotos([]);
      }
      setFeedback({
        type: 'success',
        message: initialValue ? 'Турнир обновлён' : 'Турнир создан',
      });
    } catch (submitError) {
      setFeedback({
        type: 'error',
        message: getErrorMessage(submitError, 'Не удалось сохранить турнир'),
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  const handlePhotoRemove = async (photo: MediaFileResponse) => {
    const isExistingPhoto = Boolean(initialValue?.photos.some((item) => item.id === photo.id));

    if (!isExistingPhoto) {
      await filesApi.remove(photo.id);
    }

    setPhotos((current) => current.filter((item) => item.id !== photo.id));
  };

  const handleCancel = async () => {
    const existingPhotoIds = new Set(initialValue?.photos.map((photo) => photo.id) ?? []);
    const transientPhotos = photos.filter((photo) => !existingPhotoIds.has(photo.id));

    await Promise.all(transientPhotos.map((photo) => filesApi.remove(photo.id)));
    onCancel?.();
  };

  return (
    <Card className="page__section">
      <form className="form" onSubmit={handleSubmit}>
        <div>
          <h3>{initialValue ? 'Редактирование турнира' : 'Создание турнира'}</h3>
          <p className="muted">При редактировании доступны только поля, которые backend разрешает менять после создания.</p>
        </div>
        {!initialValue ? (
          <InputField
            label="Название"
            value={form.name}
            onChange={(event) => setForm((current) => ({ ...current, name: event.target.value }))}
            required
          />
        ) : null}
        <TextAreaField
          label="Описание"
          value={form.description}
          onChange={(event) => setForm((current) => ({ ...current, description: event.target.value }))}
          required
        />
        <PhotoGalleryEditor
          label="Фото турнира"
          emptyLabel="Добавьте баннер события, площадку или кадры прошлых заездов."
          photos={photos}
          onAdd={(file) =>
            setPhotos((current) => (current.some((item) => item.id === file.id) ? current : [...current, file]))
          }
          onRemove={handlePhotoRemove}
        />
        <div className="form__row">
          <InputField
            label="Дата начала"
            type="datetime-local"
            value={form.startDate}
            onChange={(event) => setForm((current) => ({ ...current, startDate: event.target.value }))}
            required
          />
          <InputField
            label="Дата окончания"
            type="datetime-local"
            value={form.endDate}
            onChange={(event) => setForm((current) => ({ ...current, endDate: event.target.value }))}
            required
          />
        </div>
        {!initialValue ? (
          <>
            <div className="form__row">
              <SelectField
                label="Трасса"
                value={form.trackId}
                onChange={(event) => setForm((current) => ({ ...current, trackId: event.target.value }))}
              >
                {tracks.map((track) => (
                  <option key={track.id} value={track.id}>
                    {track.name} — {track.location}
                  </option>
                ))}
              </SelectField>
              <SelectField
                label="Класс автомобиля"
                value={String(form.allowedCarClass)}
                onChange={(event) =>
                  setForm((current) => ({ ...current, allowedCarClass: Number(event.target.value) }))
                }
              >
                {carReference.carClasses.map((item) => (
                  <option key={item.value} value={item.value}>
                    {carClassLabels[item.value as CarClass] ?? item.name}
                  </option>
                ))}
              </SelectField>
            </div>
            <div className="form__row">
              <InputField
                label="Минимальный стаж"
                type="number"
                min="0"
                value={form.requiredRaceCount}
                onChange={(event) =>
                  setForm((current) => ({ ...current, requiredRaceCount: Number(event.target.value) }))
                }
              />
              <InputField
                label="Нужно участников"
                type="number"
                min="1"
                value={form.requiredParticipants}
                onChange={(event) =>
                  setForm((current) => ({ ...current, requiredParticipants: Number(event.target.value) }))
                }
                required
              />
            </div>
          </>
        ) : (
          <InputField
            label="Нужно участников"
            type="number"
            min="1"
            value={form.requiredParticipants}
            onChange={(event) =>
              setForm((current) => ({ ...current, requiredParticipants: Number(event.target.value) }))
            }
            required
          />
        )}
        {feedback ? <Notice tone={feedback.type} message={feedback.message} /> : null}
        <div className="inline-actions">
          <Button type="submit" variant="primary" disabled={isSubmitting}>
            {isSubmitting ? 'Сохраняем...' : initialValue ? 'Обновить' : 'Создать турнир'}
          </Button>
          {onCancel ? (
            <Button variant="ghost" onClick={() => void handleCancel()}>
              Отмена
            </Button>
          ) : null}
        </div>
      </form>
    </Card>
  );
};
