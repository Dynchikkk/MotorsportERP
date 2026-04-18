import { useState, type FormEvent } from 'react';
import { tournamentsApi } from '@/shared/api/tournaments';
import { getErrorMessage } from '@/shared/lib/errors';
import { carClassLabels, formatCarName } from '@/shared/lib/format';
import type { CarResponse, TournamentDetailsResponse } from '@/shared/types/api';
import { Button } from '@/shared/ui/Button';
import { Card } from '@/shared/ui/Card';
import { Notice } from '@/shared/ui/Notice';
import { SelectField } from '@/shared/ui/Field';

type TournamentApplyFormProps = {
  tournament: TournamentDetailsResponse;
  cars: CarResponse[];
  onApplied: () => Promise<void>;
};

export const TournamentApplyForm = ({ tournament, cars, onApplied }: TournamentApplyFormProps) => {
  const eligibleCars = cars.filter((car) => car.carClass === tournament.allowedCarClass);
  const [carId, setCarId] = useState(eligibleCars[0]?.id ?? '');
  const [feedback, setFeedback] = useState<{ type: 'success' | 'error'; message: string } | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setIsSubmitting(true);
    setFeedback(null);

    try {
      await tournamentsApi.apply(tournament.id, {
        tournamentId: tournament.id,
        carId,
      });
      await onApplied();
      setFeedback({
        type: 'success',
        message: 'Заявка отправлена',
      });
    } catch (submitError) {
      setFeedback({
        type: 'error',
        message: getErrorMessage(submitError, 'Не удалось отправить заявку'),
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Card className="page__section">
      <form className="form" onSubmit={handleSubmit}>
        <div>
          <h3>Подать заявку</h3>
          <p className="muted">
            Допустимы машины класса {carClassLabels[tournament.allowedCarClass]}, а ваш стаж должен быть не ниже{' '}
            {tournament.requiredRaceCount} гонок.
          </p>
        </div>
        <SelectField
          label="Автомобиль"
          value={carId}
          onChange={(event) => setCarId(event.target.value)}
          hint={eligibleCars.length ? 'Можно выбрать любой подходящий автомобиль из гаража' : 'Подходящих машин пока нет'}
        >
          {eligibleCars.length ? (
            eligibleCars.map((car) => (
              <option key={car.id} value={car.id}>
                {formatCarName(car.brand, car.model, car.year)}
              </option>
            ))
          ) : (
            <option value="">Нет подходящих автомобилей</option>
          )}
        </SelectField>
        {feedback ? <Notice tone={feedback.type} message={feedback.message} /> : null}
        <Button type="submit" variant="primary" disabled={isSubmitting || !eligibleCars.length || !carId}>
          {isSubmitting ? 'Отправляем...' : 'Подать заявку'}
        </Button>
      </form>
    </Card>
  );
};
