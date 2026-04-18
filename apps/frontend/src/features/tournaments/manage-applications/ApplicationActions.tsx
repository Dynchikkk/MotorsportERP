import { useState } from 'react';
import { tournamentsApi } from '@/shared/api/tournaments';
import { getErrorMessage } from '@/shared/lib/errors';
import { Button } from '@/shared/ui/Button';

type ApplicationActionsProps = {
  applicationId: string;
  onChanged: () => Promise<void>;
};

export const ApplicationActions = ({ applicationId, onChanged }: ApplicationActionsProps) => {
  const [busy, setBusy] = useState<'approve' | 'reject' | null>(null);
  const [error, setError] = useState<string | null>(null);

  const handleApprove = async () => {
    setBusy('approve');
    setError(null);

    try {
      await tournamentsApi.approveApplication(applicationId);
      await onChanged();
    } catch (submitError) {
      setError(getErrorMessage(submitError, 'Не удалось подтвердить заявку'));
    } finally {
      setBusy(null);
    }
  };

  const handleReject = async () => {
    setBusy('reject');
    setError(null);

    try {
      await tournamentsApi.rejectApplication(applicationId);
      await onChanged();
    } catch (submitError) {
      setError(getErrorMessage(submitError, 'Не удалось отклонить заявку'));
    } finally {
      setBusy(null);
    }
  };

  return (
    <div className="stack" style={{ gap: 6 }}>
      <div className="inline-actions">
        <Button variant="primary" disabled={busy !== null} onClick={() => void handleApprove()}>
          {busy === 'approve' ? 'Подтверждаем...' : 'Подтвердить'}
        </Button>
        <Button variant="ghost" disabled={busy !== null} onClick={() => void handleReject()}>
          {busy === 'reject' ? 'Отклоняем...' : 'Отклонить'}
        </Button>
      </div>
      {error ? <span className="field__error">{error}</span> : null}
    </div>
  );
};
