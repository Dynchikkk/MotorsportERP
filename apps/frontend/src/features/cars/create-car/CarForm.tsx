import { useEffect, useState, type FormEvent } from 'react';
import { filesApi } from '@/shared/api/files';
import { carsApi } from '@/shared/api/cars';
import { getErrorMessage } from '@/shared/lib/errors';
import { carClassLabels } from '@/shared/lib/format';
import { syncMediaFiles } from '@/shared/lib/media';
import type {
  CarClass,
  CarCreateRequest,
  CarResponse,
  EnumValueResponse,
  MediaFileResponse,
} from '@/shared/types/api';
import { Button } from '@/shared/ui/Button';
import { Card } from '@/shared/ui/Card';
import { InputField, SelectField, TextAreaField } from '@/shared/ui/Field';
import { Notice } from '@/shared/ui/Notice';
import { PhotoGalleryEditor } from '@/shared/ui/PhotoGalleryEditor';

type CarFormProps = {
  carClasses: EnumValueResponse[];
  initialValue?: CarResponse;
  onSuccess: () => Promise<void>;
  onCancel?: () => void;
};

export const CarForm = ({ carClasses, initialValue, onSuccess, onCancel }: CarFormProps) => {
  const [form, setForm] = useState<CarCreateRequest>({
    brand: initialValue?.brand ?? '',
    model: initialValue?.model ?? '',
    year: initialValue?.year ?? new Date().getFullYear(),
    description: initialValue?.description ?? '',
    carClass: initialValue?.carClass ?? carClasses[0]?.value ?? 0,
  });
  const [photos, setPhotos] = useState<MediaFileResponse[]>(initialValue?.photos ?? []);
  const [feedback, setFeedback] = useState<{ type: 'success' | 'error'; message: string } | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    setForm({
      brand: initialValue?.brand ?? '',
      model: initialValue?.model ?? '',
      year: initialValue?.year ?? new Date().getFullYear(),
      description: initialValue?.description ?? '',
      carClass: initialValue?.carClass ?? carClasses[0]?.value ?? 0,
    });
    setPhotos(initialValue?.photos ?? []);
  }, [carClasses, initialValue]);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setIsSubmitting(true);
    setFeedback(null);

    try {
      if (initialValue) {
        await carsApi.update(initialValue.id, form);
        await syncMediaFiles({
          currentPhotos: initialValue.photos,
          nextPhotos: photos,
          addPhoto: (photoId) => carsApi.addPhoto(initialValue.id, photoId),
          removePhoto: (photoId) => carsApi.removePhoto(initialValue.id, photoId),
        });
      } else {
        const createdCarId = await carsApi.create(form);
        await Promise.all(photos.map((photo) => carsApi.addPhoto(createdCarId, photo.id)));
      }

      await onSuccess();

      if (!initialValue) {
        setForm({
          brand: '',
          model: '',
          year: new Date().getFullYear(),
          description: '',
          carClass: carClasses[0]?.value ?? 0,
        });
        setPhotos([]);
      }

      setFeedback({
        type: 'success',
        message: initialValue ? 'Автомобиль обновлён' : 'Автомобиль добавлен',
      });
    } catch (submitError) {
      setFeedback({
        type: 'error',
        message: getErrorMessage(submitError, 'Не удалось сохранить автомобиль'),
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
          <h3>{initialValue ? 'Редактирование автомобиля' : 'Добавление автомобиля'}</h3>
          <p className="muted">Автомобиль появится в вашем гараже и станет доступен для подачи заявки на турнир.</p>
        </div>
        <div className="form__row">
          <InputField
            label="Марка"
            value={form.brand}
            onChange={(event) => setForm((current) => ({ ...current, brand: event.target.value }))}
            required
          />
          <InputField
            label="Модель"
            value={form.model}
            onChange={(event) => setForm((current) => ({ ...current, model: event.target.value }))}
            required
          />
        </div>
        <div className="form__row">
          <InputField
            label="Год выпуска"
            type="number"
            min="1950"
            max="2100"
            value={form.year}
            onChange={(event) => setForm((current) => ({ ...current, year: Number(event.target.value) }))}
            required
          />
          <SelectField
            label="Класс"
            value={String(form.carClass)}
            onChange={(event) => setForm((current) => ({ ...current, carClass: Number(event.target.value) }))}
          >
            {carClasses.map((item) => (
              <option key={item.value} value={item.value}>
                {carClassLabels[item.value as CarClass] ?? item.name}
              </option>
            ))}
          </SelectField>
        </div>
        <TextAreaField
          label="Описание"
          value={form.description ?? ''}
          onChange={(event) => setForm((current) => ({ ...current, description: event.target.value }))}
          placeholder="Подготовка, особенности, конфигурация"
        />
        <PhotoGalleryEditor
          label="Фотографии автомобиля"
          emptyLabel="Добавьте экстерьер, салон или фото подготовки."
          photos={photos}
          onAdd={(file) =>
            setPhotos((current) => (current.some((item) => item.id === file.id) ? current : [...current, file]))
          }
          onRemove={handlePhotoRemove}
        />
        {feedback ? <Notice tone={feedback.type} message={feedback.message} /> : null}
        <div className="inline-actions">
          <Button type="submit" variant="primary" disabled={isSubmitting}>
            {isSubmitting ? 'Сохраняем...' : initialValue ? 'Сохранить' : 'Добавить'}
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
