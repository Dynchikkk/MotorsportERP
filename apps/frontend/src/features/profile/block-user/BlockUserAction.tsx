import { useState } from 'react';
import { usersApi } from '@/shared/api/users';
import { getErrorMessage } from '@/shared/lib/errors';
import { Button } from '@/shared/ui/Button';

type BlockUserActionProps = {
  userId: string;
  isBlocked: boolean;
  onChanged: () => Promise<void>;
};

export const BlockUserAction = ({ userId, isBlocked, onChanged }: BlockUserActionProps) => {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleClick = async () => {
    setIsSubmitting(true);
    setError(null);

    try {
      await usersApi.setBlockStatus(userId, !isBlocked);
      await onChanged();
    } catch (submitError) {
      setError(getErrorMessage(submitError, 'Не удалось изменить статус блокировки'));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="stack" style={{ gap: 6 }}>
      <Button variant={isBlocked ? 'secondary' : 'danger'} onClick={() => void handleClick()} disabled={isSubmitting}>
        {isSubmitting ? 'Обновляем...' : isBlocked ? 'Разблокировать' : 'Заблокировать'}
      </Button>
      {error ? <span className="field__error">{error}</span> : null}
    </div>
  );
};
