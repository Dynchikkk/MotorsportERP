import { useState, type FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '@/app/providers/AuthProvider';
import { PATHS } from '@/shared/config/paths';
import { getErrorMessage } from '@/shared/lib/errors';
import { Button } from '@/shared/ui/Button';
import { Card } from '@/shared/ui/Card';
import { InputField } from '@/shared/ui/Field';
import { Notice } from '@/shared/ui/Notice';

export const RegisterForm = () => {
  const navigate = useNavigate();
  const { register } = useAuth();
  const [values, setValues] = useState({
    nickname: '',
    email: '',
    password: '',
  });
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setIsSubmitting(true);
    setError(null);

    try {
      await register(values);
      navigate(PATHS.USERS.PROFILE, { replace: true });
    } catch (submitError) {
      setError(getErrorMessage(submitError, 'Не удалось зарегистрироваться'));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Card className="page__section">
      <form className="form" onSubmit={handleSubmit}>
        <InputField
          label="Никнейм"
          value={values.nickname}
          onChange={(event) => setValues((current) => ({ ...current, nickname: event.target.value }))}
          placeholder="Например, ApexDriver"
          required
        />
        <InputField
          label="Электронная почта"
          type="email"
          value={values.email}
          onChange={(event) => setValues((current) => ({ ...current, email: event.target.value }))}
          placeholder="pilot@motorsport.local"
          required
        />
        <InputField
          label="Пароль"
          type="password"
          value={values.password}
          onChange={(event) => setValues((current) => ({ ...current, password: event.target.value }))}
          placeholder="Введите пароль"
          required
        />
        {error ? <Notice tone="error" message={error} /> : null}
        <div className="toolbar">
          <Button type="submit" variant="primary" disabled={isSubmitting}>
            {isSubmitting ? 'Создаём профиль...' : 'Создать аккаунт'}
          </Button>
          <span className="muted">
            Уже зарегистрированы? <Link to={PATHS.AUTH.LOGIN}>Войти</Link>
          </span>
        </div>
      </form>
    </Card>
  );
};
