import { Outlet, useLocation } from 'react-router-dom';
import { Header } from '@/widgets/header/Header';
import { Sidebar } from '@/widgets/sidebar/Sidebar';

const AUTH_PATHS = ['/login', '/register'];

export const AppLayout = () => {
  const location = useLocation();
  const isAuthPage = AUTH_PATHS.includes(location.pathname);

  if (isAuthPage) {
    return (
      <div className="auth-layout">
        <Header compact />
        <main className="auth-layout__content">
          <Outlet />
        </main>
      </div>
    );
  }

  return (
    <div className="app-shell">
      <Sidebar />
      <div className="app-shell__body">
        <Header />
        <main className="app-shell__content">
          <Outlet />
        </main>
      </div>
    </div>
  );
};
