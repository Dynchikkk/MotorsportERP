import { createBrowserRouter } from 'react-router-dom';
import { AppLayout } from './ui/AppLayout';
import { ProtectedRoute } from './ui/ProtectedRoute';
import { PATHS } from '@/shared/config/paths';
import { GaragePage } from '@/pages/garage';
import { HomePage } from '@/pages/home';
import { LoginPage } from '@/pages/login';
import { ProfilePage } from '@/pages/profile';
import { RegisterPage } from '@/pages/register';
import { TrackDetailsPage } from '@/pages/track-details';
import { TracksPage } from '@/pages/tracks';
import { TournamentDetailsPage } from '@/pages/tournament-details';
import { TournamentsPage } from '@/pages/tournaments';
import { PublicUserPage, UsersPage } from '@/pages/users';

export const appRouter = createBrowserRouter([
  {
    path: PATHS.HOME,
    element: <AppLayout />,
    children: [
      {
        index: true,
        element: <HomePage />,
      },
      {
        path: PATHS.AUTH.LOGIN,
        element: <LoginPage />,
      },
      {
        path: PATHS.AUTH.REGISTER,
        element: <RegisterPage />,
      },
      {
        path: PATHS.TOURNAMENTS.ROOT,
        element: <TournamentsPage />,
      },
      {
        path: PATHS.TOURNAMENTS.DETAILS(),
        element: <TournamentDetailsPage />,
      },
      {
        path: PATHS.TRACKS.ROOT,
        element: <TracksPage />,
      },
      {
        path: PATHS.TRACKS.DETAILS(),
        element: <TrackDetailsPage />,
      },
      {
        path: PATHS.USERS.ROOT,
        element: <UsersPage />,
      },
      {
        path: PATHS.USERS.DETAILS(),
        element: <PublicUserPage />,
      },
      {
        element: <ProtectedRoute />,
        children: [
          {
            path: PATHS.USERS.PROFILE,
            element: <ProfilePage />,
          },
          {
            path: PATHS.USERS.GARAGE,
            element: <GaragePage />,
          },
        ],
      },
    ],
  },
]);
