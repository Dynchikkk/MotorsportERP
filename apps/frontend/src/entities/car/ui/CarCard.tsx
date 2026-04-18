import type { ReactNode } from 'react';
import { carClassLabels, formatCarName } from '@/shared/lib/format';
import { resolveMediaUrl } from '@/shared/lib/media';
import type { CarResponse } from '@/shared/types/api';
import { Card } from '@/shared/ui/Card';
import { Pill } from '@/shared/ui/Pill';

type CarCardProps = {
  car: CarResponse;
  actions?: ReactNode;
};

export const CarCard = ({ car, actions }: CarCardProps) => {
  return (
    <Card className="entity-card">
      <div className="entity-card__top">
        <div>
          <h3 className="entity-card__title">{formatCarName(car.brand, car.model, car.year)}</h3>
          <p className="muted">{car.description || 'Без дополнительного описания'}</p>
        </div>
        <Pill tone="accent">{carClassLabels[car.carClass]}</Pill>
      </div>
      {car.photos.length > 0 ? (
        <div className="photo-strip">
          {car.photos.map((photo) => (
            <img key={photo.id} src={resolveMediaUrl(photo.url) ?? ''} alt={car.model} />
          ))}
        </div>
      ) : null}
      {actions ? <div className="inline-actions">{actions}</div> : null}
    </Card>
  );
};
