import { useEffect, useState, type FormEvent } from 'react';
import { usersApi } from '@/shared/api/users';
import { getErrorMessage } from '@/shared/lib/errors';
import type { MediaFileResponse, UserProfileResponse } from '@/shared/types/api';
import { Avatar } from '@/shared/ui/Avatar';
import { Button } from '@/shared/ui/Button';
import { Card } from '@/shared/ui/Card';
import { InputField, TextAreaField } from '@/shared/ui/Field';
import { Notice } from '@/shared/ui/Notice';
import { PhotoUploader } from '@/shared/ui/PhotoUploader';

type UpdateProfileFormProps = {
  profile: UserProfileResponse;
  onUpdated: () => Promise<void>;
};

export const UpdateProfileForm = ({ profile, onUpdated }: UpdateProfileFormProps) => {
  const [nickname, setNickname] = useState(profile.nickname);
  const [bio, setBio] = useState(profile.bio ?? '');
  const [avatar, setAvatar] = useState<MediaFileResponse | null>(profile.avatar ?? null);
  const [feedback, setFeedback] = useState<{ type: 'success' | 'error'; message: string } | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    setNickname(profile.nickname);
    setBio(profile.bio ?? '');
    setAvatar(profile.avatar ?? null);
  }, [profile.avatar, profile.bio, profile.nickname]);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setIsSubmitting(true);
    setFeedback(null);

    try {
      await usersApi.updateProfile({
        nickname,
        bio: bio || null,
        avatar,
      });
      await onUpdated();
      setFeedback({
        type: 'success',
        message: 'Профиль обновлён',
      });
    } catch (submitError) {
      setFeedback({
        type: 'error',
        message: getErrorMessage(submitError, 'Не удалось сохранить профиль'),
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Card className="page__section">
      <div className="toolbar">
        <div>
          <h2>Редактирование профиля</h2>
          <p className="muted">Никнейм, био и аватар используются в публичных карточках и в турнирной истории.</p>
        </div>
        <Avatar name={nickname} avatar={avatar} size={72} />
      </div>
      <form className="form" onSubmit={handleSubmit}>
        <div className="form__row">
          <InputField
            label="Никнейм"
            value={nickname}
            onChange={(event) => setNickname(event.target.value)}
            required
          />
          <InputField label="E-mail" value={profile.email} disabled />
        </div>
        <TextAreaField
          label="О себе"
          value={bio}
          onChange={(event) => setBio(event.target.value)}
          placeholder="Коротко о своём опыте, дисциплине или технике"
        />
        <PhotoUploader label="Загрузить аватар" onUploaded={(file) => setAvatar(file)} />
        {feedback ? <Notice tone={feedback.type} message={feedback.message} /> : null}
        <div className="inline-actions">
          <Button type="submit" variant="primary" disabled={isSubmitting}>
            {isSubmitting ? 'Сохраняем...' : 'Сохранить изменения'}
          </Button>
          {avatar ? (
            <Button variant="ghost" onClick={() => setAvatar(null)}>
              Убрать аватар
            </Button>
          ) : null}
        </div>
      </form>
    </Card>
  );
};
