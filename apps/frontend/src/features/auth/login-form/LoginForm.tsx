import { useState, type FormEvent } from 'react';
import type { Location } from 'react-router-dom';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '@/app/providers/AuthProvider';
import { PATHS } from '@/shared/config/paths';
import { getErrorMessage } from '@/shared/lib/errors';
import { Button } from '@/shared/ui/Button';
import { Card } from '@/shared/ui/Card';
import { InputField } from '@/shared/ui/Field';
import { Notice } from '@/shared/ui/Notice';

export const LoginForm = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { login } = useAuth();
  const [values, setValues] = useState({
    email: '',
    password: '',
  });
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const from =
    ((location.state as { from?: Location } | null)?.from?.pathname as string | undefined) ?? PATHS.HOME;

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setIsSubmitting(true);
    setError(null);

    try {
      await login(values);
      navigate(from, { replace: true });
    } catch (submitError) {
      setError(getErrorMessage(submitError, 'Не удалось войти'));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Card className="page__section">
      <form className="form" onSubmit={handleSubmit}>
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
            {isSubmitting ? 'Входим...' : 'Войти'}
          </Button>
          <span className="muted">
            Нет аккаунта? <Link to={PATHS.AUTH.REGISTER}>Зарегистрироваться</Link>
          </span>
        </div>
      </form>
    </Card>
  );
};
