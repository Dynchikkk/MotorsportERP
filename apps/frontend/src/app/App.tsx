import { RouterProvider } from 'react-router-dom';
import { AppProviders } from './providers/AppProviders';
import { appRouter } from './router/router';

export const App = () => {
  return (
    <AppProviders>
      <RouterProvider router={appRouter} />
    </AppProviders>
  );
};
