import { useEffect, useState, type FormEvent } from 'react';
import { filesApi } from '@/shared/api/files';
import { tracksApi } from '@/shared/api/tracks';
import { getErrorMessage } from '@/shared/lib/errors';
import { trackStatusLabels } from '@/shared/lib/format';
import { syncMediaFiles } from '@/shared/lib/media';
import type {
  EnumValueResponse,
  MediaFileResponse,
  TrackCreateRequest,
  TrackDetailsResponse,
  TrackStatus,
} from '@/shared/types/api';
import { Button } from '@/shared/ui/Button';
import { Card } from '@/shared/ui/Card';
import { InputField, SelectField } from '@/shared/ui/Field';
import { Notice } from '@/shared/ui/Notice';
import { PhotoGalleryEditor } from '@/shared/ui/PhotoGalleryEditor';

type TrackFormProps = {
  initialValue?: TrackDetailsResponse;
  allowStatusSelection?: boolean;
  statusOptions: EnumValueResponse[];
  onSuccess: () => Promise<void>;
  onCancel?: () => void;
};

export const TrackForm = ({
  initialValue,
  allowStatusSelection = false,
  statusOptions,
  onSuccess,
  onCancel,
}: TrackFormProps) => {
  const [form, setForm] = useState<TrackCreateRequest>({
    name: initialValue?.name ?? '',
    location: initialValue?.location ?? '',
    status: initialValue?.status ?? undefined,
  });
  const [photos, setPhotos] = useState<MediaFileResponse[]>(initialValue?.photos ?? []);
  const [feedback, setFeedback] = useState<{ type: 'success' | 'error'; message: string } | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    setForm({
      name: initialValue?.name ?? '',
      location: initialValue?.location ?? '',
      status: initialValue?.status ?? undefined,
    });
    setPhotos(initialValue?.photos ?? []);
  }, [initialValue]);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setIsSubmitting(true);
    setFeedback(null);

    try {
      if (initialValue) {
        await tracksApi.update(initialValue.id, {
          name: form.name,
          location: form.location,
        });
        await syncMediaFiles({
          currentPhotos: initialValue.photos,
          nextPhotos: photos,
          addPhoto: (photoId) => tracksApi.addPhoto(initialValue.id, photoId),
          removePhoto: (photoId) => tracksApi.removePhoto(initialValue.id, photoId),
        });
      } else {
        const createdTrackId = await tracksApi.create(form);
        await Promise.all(photos.map((photo) => tracksApi.addPhoto(createdTrackId, photo.id)));
      }

      await onSuccess();
      if (!initialValue) {
        setForm({
          name: '',
          location: '',
          status: undefined,
        });
        setPhotos([]);
      }

      setFeedback({
        type: 'success',
        message: initialValue ? 'Трасса обновлена' : 'Трасса добавлена',
      });
    } catch (submitError) {
      setFeedback({
        type: 'error',
        message: getErrorMessage(submitError, 'Не удалось сохранить трассу'),
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
          <h3>{initialValue ? 'Редактирование трассы' : 'Добавление трассы'}</h3>
          <p className="muted">Новые трассы от сообщества по умолчанию попадают в статус неофициальных.</p>
        </div>
        <InputField
          label="Название"
          value={form.name}
          onChange={(event) => setForm((current) => ({ ...current, name: event.target.value }))}
          required
        />
        <InputField
          label="Локация"
          value={form.location}
          onChange={(event) => setForm((current) => ({ ...current, location: event.target.value }))}
          required
        />
        <PhotoGalleryEditor
          label="Фотографии трассы"
          emptyLabel="Добавьте схему, общий вид или ориентиры площадки."
          photos={photos}
          onAdd={(file) =>
            setPhotos((current) => (current.some((item) => item.id === file.id) ? current : [...current, file]))
          }
          onRemove={handlePhotoRemove}
        />
        {allowStatusSelection ? (
          <SelectField
            label="Статус"
            value={String(form.status ?? statusOptions[0]?.value ?? 0)}
            onChange={(event) =>
              setForm((current) => ({ ...current, status: Number(event.target.value) as TrackStatus }))
            }
          >
            {statusOptions.map((item) => (
              <option key={item.value} value={item.value}>
                {trackStatusLabels[item.value as TrackStatus] ?? item.name}
              </option>
            ))}
          </SelectField>
        ) : null}
        {feedback ? <Notice tone={feedback.type} message={feedback.message} /> : null}
        <div className="inline-actions">
          <Button type="submit" variant="primary" disabled={isSubmitting}>
            {isSubmitting ? 'Сохраняем...' : initialValue ? 'Сохранить' : 'Добавить трассу'}
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
