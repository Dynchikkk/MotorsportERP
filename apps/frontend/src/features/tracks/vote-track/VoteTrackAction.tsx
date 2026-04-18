import { useState } from 'react';
import { tracksApi } from '@/shared/api/tracks';
import { getErrorMessage } from '@/shared/lib/errors';
import { Button } from '@/shared/ui/Button';

type VoteTrackActionProps = {
  trackId: string;
  onVoted: () => Promise<void>;
};

export const VoteTrackAction = ({ trackId, onVoted }: VoteTrackActionProps) => {
  const [busy, setBusy] = useState<'positive' | 'negative' | null>(null);
  const [error, setError] = useState<string | null>(null);

  const handleVote = async (isPositive: boolean) => {
    setBusy(isPositive ? 'positive' : 'negative');
    setError(null);

    try {
      await tracksApi.vote(trackId, isPositive);
      await onVoted();
    } catch (voteError) {
      setError(getErrorMessage(voteError, 'Не удалось отправить голос'));
    } finally {
      setBusy(null);
    }
  };

  return (
    <div className="stack">
      <div className="inline-actions">
        <Button variant="primary" disabled={busy !== null} onClick={() => void handleVote(true)}>
          {busy === 'positive' ? 'Отправляем...' : 'Поддержать трассу'}
        </Button>
        <Button variant="ghost" disabled={busy !== null} onClick={() => void handleVote(false)}>
          {busy === 'negative' ? 'Отправляем...' : 'Голос против'}
        </Button>
      </div>
      {error ? <span className="field__error">{error}</span> : null}
    </div>
  );
};
