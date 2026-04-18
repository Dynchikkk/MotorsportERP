import { Navigate } from 'react-router-dom';
import { useAuth } from '@/app/providers/AuthProvider';
import { LoginForm } from '@/features/auth/login-form/LoginForm';
import { PATHS } from '@/shared/config/paths';
import { PageHeader } from '@/shared/ui/PageHeader';

export const LoginPage = () => {
  const { isAuthenticated } = useAuth();

  if (isAuthenticated) {
    return <Navigate to={PATHS.USERS.PROFILE} replace />;
  }

  return (
    <div className="page">
      <PageHeader
        eyebrow="Авторизация"
        title="Вход в Motorsport ERP"
        description="После входа откроются личный кабинет, гараж, заявки на турниры и, при наличии ролей, инструменты модерации."
      />
      <LoginForm />
    </div>
  );
};
