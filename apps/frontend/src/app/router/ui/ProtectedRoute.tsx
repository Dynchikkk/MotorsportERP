import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuth } from '@/app/providers/AuthProvider';
import { Loader } from '@/shared/ui/Loader';
import { PATHS } from '@/shared/config/paths';

export const ProtectedRoute = () => {
  const { isAuthenticated, isBootstrapping } = useAuth();
  const location = useLocation();

  if (isBootstrapping) {
    return <Loader label="Подготавливаем личный кабинет" fullHeight />;
  }

  if (!isAuthenticated) {
    return <Navigate to={PATHS.AUTH.LOGIN} replace state={{ from: location }} />;
  }

  return <Outlet />;
};
