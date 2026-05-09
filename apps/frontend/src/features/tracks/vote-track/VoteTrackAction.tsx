import { useState } from 'react';
import { tracksApi } from '@/shared/api/tracks';
import { getErrorMessage } from '@/shared/lib/errors';
import type { TrackCurrentUserVoteSummary } from '@/shared/types/api';
import { Button } from '@/shared/ui/Button';

type VoteTrackActionProps = {
  trackId: string;
  onVoted: () => Promise<void>;
  currentUserVote?: TrackCurrentUserVoteSummary | null;
};

export const VoteTrackAction = ({ trackId, onVoted, currentUserVote }: VoteTrackActionProps) => {
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
      <p className="muted">
        {currentUserVote?.hasVoted && currentUserVote.isPositive === true
          ? 'Вы уже поддержали эту трассу. Ниже можно изменить голос.'
          : currentUserVote?.hasVoted && currentUserVote.isPositive === false
            ? 'Ваш голос: не поддерживаю. Ниже можно изменить мнение.'
            : 'Ваш голос помогает вынести трассу в подтверждённые после порога поддержки.'}
      </p>
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
