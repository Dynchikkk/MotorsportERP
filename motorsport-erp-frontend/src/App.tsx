import { RouterProvider } from 'react-router-dom';
import { useEffect } from 'react';
import { appRouter } from '@/app/router/appRouter';

export const App = () => {
  useEffect(() => {
    const handleAuthError = () => {
      // Принудительно переходим на логин через роутер без перезагрузки
      appRouter.navigate('/login');
    };

    window.addEventListener('unauthorized', handleAuthError);
    return () => window.removeEventListener('unauthorized', handleAuthError);
  }, []);

  return <RouterProvider router={appRouter} />;
};