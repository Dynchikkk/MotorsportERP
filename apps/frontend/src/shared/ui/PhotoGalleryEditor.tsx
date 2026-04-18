import { resolveMediaUrl } from '@/shared/lib/media';
import type { MediaFileResponse } from '@/shared/types/api';
import { Button } from './Button';
import { PhotoUploader } from './PhotoUploader';

type PhotoGalleryEditorProps = {
  photos: MediaFileResponse[];
  label?: string;
  emptyLabel?: string;
  onAdd: (file: MediaFileResponse) => Promise<void> | void;
  onRemove: (photo: MediaFileResponse) => Promise<void> | void;
};

export const PhotoGalleryEditor = ({
  photos,
  label = 'Фотографии',
  emptyLabel = 'Фотографии пока не добавлены',
  onAdd,
  onRemove,
}: PhotoGalleryEditorProps) => {
  return (
    <div className="stack">
      <div>
        <strong>{label}</strong>
      </div>
      <PhotoUploader label="Загрузить фото" onUploaded={onAdd} />
      {photos.length ? (
        <div className="gallery-editor">
          {photos.map((photo) => (
            <div key={photo.id} className="gallery-editor__item">
              <img src={resolveMediaUrl(photo.url) ?? ''} alt="" className="photo-frame" />
              <Button variant="ghost" onClick={() => void onRemove(photo)}>
                Убрать
              </Button>
            </div>
          ))}
        </div>
      ) : (
        <span className="muted">{emptyLabel}</span>
      )}
    </div>
  );
};
