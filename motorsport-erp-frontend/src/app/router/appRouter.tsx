import { createBrowserRouter, Navigate } from 'react-router-dom';
import { PATHS } from '@/shared/config/paths';

// Временные заглушки. Позже мы заменим их на реальные импорты из папки pages/
const HomePage = () => <div className="p-4">Главная страница</div>;
const LoginPage = () => <div className="p-4">Страница входа</div>;
const TournamentsPage = () => <div className="p-4">Список турниров</div>;

export const appRouter = createBrowserRouter([
  {
    path: PATHS.HOME,
    element: <HomePage />,
  },
  {
    path: PATHS.AUTH.LOGIN,
    element: <LoginPage />,
  },
  {
    path: PATHS.TOURNAMENTS.ROOT,
    element: <TournamentsPage />,
  },
  {
    path: '*',
    element: <Navigate to={PATHS.HOME} replace />,
  },
]);