import { Navigate, Outlet } from 'react-router-dom';
import { PATHS } from '@/shared/config/paths';

export const ProtectedRoute = () => {
  const isAuth = !!localStorage.getItem('accessToken');

  if (!isAuth) {
    return <Navigate to={PATHS.AUTH.LOGIN} replace />;
  }

  return <Outlet />;
};