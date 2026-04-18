import { useState, type FormEvent } from 'react';
import { tournamentsApi } from '@/shared/api/tournaments';
import { getErrorMessage } from '@/shared/lib/errors';
import type { TournamentApplicationResponse } from '@/shared/types/api';
import { Button } from '@/shared/ui/Button';
import { Card } from '@/shared/ui/Card';
import { InputField, SelectField } from '@/shared/ui/Field';
import { Notice } from '@/shared/ui/Notice';

type AddResultFormProps = {
  tournamentId: string;
  participants: TournamentApplicationResponse[];
  existingResultUserIds: string[];
  onAdded: () => Promise<void>;
};

export const AddResultForm = ({
  tournamentId,
  participants,
  existingResultUserIds,
  onAdded,
}: AddResultFormProps) => {
  const availableParticipants = participants.filter((participant) => !existingResultUserIds.includes(participant.user.id));
  const [userId, setUserId] = useState(availableParticipants[0]?.user.id ?? '');
  const [position, setPosition] = useState(existingResultUserIds.length + 1 || 1);
  const [bestLapTime, setBestLapTime] = useState('');
  const [feedback, setFeedback] = useState<{ type: 'success' | 'error'; message: string } | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setIsSubmitting(true);
    setFeedback(null);

    try {
      await tournamentsApi.addResult(tournamentId, {
        userId,
        position,
        bestLapTime: bestLapTime || null,
      });
      await onAdded();
      setFeedback({
        type: 'success',
        message: 'Результат добавлен',
      });
    } catch (submitError) {
      setFeedback({
        type: 'error',
        message: getErrorMessage(submitError, 'Не удалось добавить результат'),
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Card className="page__section">
      <form className="form" onSubmit={handleSubmit}>
        <div>
          <h3>Добавить результат</h3>
          <p className="muted">Результаты можно заводить только для одобренных участников активного турнира.</p>
        </div>
        <SelectField
          label="Участник"
          value={userId}
          onChange={(event) => setUserId(event.target.value)}
          hint={availableParticipants.length ? undefined : 'Все результаты уже заполнены'}
        >
          {availableParticipants.length ? (
            availableParticipants.map((participant) => (
              <option key={participant.user.id} value={participant.user.id}>
                {participant.user.nickname} — {participant.car.brand} {participant.car.model}
              </option>
            ))
          ) : (
            <option value="">Нет доступных участников</option>
          )}
        </SelectField>
        <div className="form__row">
          <InputField
            label="Позиция"
            type="number"
            min="1"
            value={position}
            onChange={(event) => setPosition(Number(event.target.value))}
            required
          />
          <InputField
            label="Лучший круг"
            value={bestLapTime}
            onChange={(event) => setBestLapTime(event.target.value)}
            placeholder="00:01:45"
          />
        </div>
        {feedback ? <Notice tone={feedback.type} message={feedback.message} /> : null}
        <Button type="submit" variant="primary" disabled={isSubmitting || !availableParticipants.length || !userId}>
          {isSubmitting ? 'Сохраняем...' : 'Добавить результат'}
        </Button>
      </form>
    </Card>
  );
};
