import { Navigate } from 'react-router-dom';
import { useAuth } from '@/app/providers/AuthProvider';
import { RegisterForm } from '@/features/auth/register-form/RegisterForm';
import { PATHS } from '@/shared/config/paths';
import { PageHeader } from '@/shared/ui/PageHeader';

export const RegisterPage = () => {
  const { isAuthenticated } = useAuth();

  if (isAuthenticated) {
    return <Navigate to={PATHS.USERS.PROFILE} replace />;
  }

  return (
    <div className="page">
      <PageHeader
        eyebrow="Регистрация"
        title="Создать профиль пилота"
        description="Новый пользователь автоматически получает роль гонщика. Остальные роли открываются только по правилам backend."
      />
      <RegisterForm />
    </div>
  );
};
