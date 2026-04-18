import { NavLink } from 'react-router-dom';
import { useAuth } from '@/app/providers/AuthProvider';
import { personalNavigation, primaryNavigation } from '@/shared/config/navigation';
import { isModeratorOrAbove } from '@/shared/lib/roles';

export const Sidebar = () => {
  const { currentUser, isAuthenticated } = useAuth();

  return (
    <aside className="sidebar">
      <div className="sidebar__inner">
        <section className="sidebar__section">
          <p className="sidebar__label">Навигация</p>
          <nav className="sidebar__nav">
            {primaryNavigation.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                className={({ isActive }) => `sidebar__link${isActive ? ' active' : ''}`}
              >
                <span className="subtle">{item.icon}</span>
                {item.label}
              </NavLink>
            ))}
          </nav>
        </section>

        {isAuthenticated ? (
          <section className="sidebar__section">
            <p className="sidebar__label">Личное</p>
            <nav className="sidebar__nav">
              {personalNavigation.map((item) => (
                <NavLink
                  key={item.to}
                  to={item.to}
                  className={({ isActive }) => `sidebar__link${isActive ? ' active' : ''}`}
                >
                  <span className="subtle">{item.icon}</span>
                  {item.label}
                </NavLink>
              ))}
            </nav>
          </section>
        ) : null}

        <section className="sidebar__section">
          <p className="sidebar__label">Система</p>
          <div className="stack" style={{ gap: 10 }}>
            <div>
              <strong>{currentUser ? currentUser.nickname : 'Гость'}</strong>
              <p className="muted" style={{ margin: '6px 0 0' }}>
                {currentUser
                  ? `${currentUser.raceCount} гонок, ${
                      isModeratorOrAbove(currentUser.roles) ? 'расширенные права' : 'роль гонщика'
                    }`
                  : 'Просматривайте турниры, трассы и профили даже без входа'}
              </p>
            </div>
          </div>
        </section>
      </div>
    </aside>
  );
};
