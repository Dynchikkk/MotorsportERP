import { useState } from 'react';
import { carsApi } from '@/shared/api/cars';
import { getErrorMessage } from '@/shared/lib/errors';
import { Button } from '@/shared/ui/Button';

type DeleteCarButtonProps = {
  carId: string;
  onDeleted: () => Promise<void>;
};

export const DeleteCarButton = ({ carId, onDeleted }: DeleteCarButtonProps) => {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleDelete = async () => {
    if (!window.confirm('Удалить автомобиль из гаража?')) {
      return;
    }

    setIsSubmitting(true);
    setError(null);

    try {
      await carsApi.remove(carId);
      await onDeleted();
    } catch (submitError) {
      setError(getErrorMessage(submitError, 'Не удалось удалить автомобиль'));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="stack" style={{ gap: 6 }}>
      <Button variant="danger" onClick={() => void handleDelete()} disabled={isSubmitting}>
        {isSubmitting ? 'Удаляем...' : 'Удалить'}
      </Button>
      {error ? <span className="field__error">{error}</span> : null}
    </div>
  );
};
