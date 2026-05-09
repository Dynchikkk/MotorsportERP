import { resolveMediaUrl } from '@/shared/lib/media';
import type { CarResponse } from '@/shared/types/api';

type CarPhotoThumbProps = {
  car: CarResponse;
  alt?: string;
  className?: string;
};

export const CarPhotoThumb = ({ car, alt, className }: CarPhotoThumbProps) => {
  const photo = car.photos[0];
  if (!photo) {
    return null;
  }

  return (
    <img
      className={className ?? 'photo-frame'}
      src={resolveMediaUrl(photo.url) ?? ''}
      alt={alt ?? `${car.brand} ${car.model}`}
    />
  );
};
