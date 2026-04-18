import { useState } from 'react';
import { usersApi } from '@/shared/api/users';
import { getErrorMessage } from '@/shared/lib/errors';
import { UserRole } from '@/shared/types/api';
import { Button } from '@/shared/ui/Button';
import { SelectField } from '@/shared/ui/Field';
import { Notice } from '@/shared/ui/Notice';

type AssignRoleActionProps = {
  userId: string;
  onAssigned: () => Promise<void>;
};

const assignableRoles = [
  { value: UserRole.Organizer, label: 'Организатор' },
  { value: UserRole.Moderator, label: 'Модератор' },
] as const;

export const AssignRoleAction = ({ userId, onAssigned }: AssignRoleActionProps) => {
  const [role, setRole] = useState<UserRole>(UserRole.Organizer);
  const [feedback, setFeedback] = useState<{ type: 'success' | 'error'; message: string } | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleAssign = async () => {
    setIsSubmitting(true);
    setFeedback(null);

    try {
      await usersApi.assignRole(userId, role);
      await onAssigned();
      setFeedback({
        type: 'success',
        message: 'Роль назначена',
      });
    } catch (assignError) {
      setFeedback({
        type: 'error',
        message: getErrorMessage(assignError, 'Не удалось назначить роль'),
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="stack">
      <SelectField
        label="Назначить роль"
        value={String(role)}
        onChange={(event) => setRole(Number(event.target.value) as UserRole)}
      >
        {assignableRoles.map((item) => (
          <option key={item.value} value={item.value}>
            {item.label}
          </option>
        ))}
      </SelectField>
      <Button variant="secondary" onClick={() => void handleAssign()} disabled={isSubmitting}>
        {isSubmitting ? 'Назначаем...' : 'Выдать роль'}
      </Button>
      {feedback ? <Notice tone={feedback.type} message={feedback.message} /> : null}
    </div>
  );
};
