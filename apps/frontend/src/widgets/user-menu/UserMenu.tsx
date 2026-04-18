import { Link } from 'react-router-dom';
import { useAuth } from '@/app/providers/AuthProvider';
import { PATHS } from '@/shared/config/paths';
import { Avatar } from '@/shared/ui/Avatar';
import { Button } from '@/shared/ui/Button';

export const UserMenu = () => {
  const { currentUser, isAuthenticated, logout } = useAuth();

  if (!isAuthenticated || !currentUser) {
    return (
      <div className="cluster">
        <Link to={PATHS.AUTH.LOGIN}>
          <Button variant="ghost">Войти</Button>
        </Link>
        <Link to={PATHS.AUTH.REGISTER}>
          <Button variant="primary">Регистрация</Button>
        </Link>
      </div>
    );
  }

  return (
    <div className="cluster">
      <Link className="user-badge" to={PATHS.USERS.PROFILE}>
        <Avatar name={currentUser.nickname} avatar={currentUser.avatar} />
        <div className="stack" style={{ gap: 4 }}>
          <strong>{currentUser.nickname}</strong>
          <span className="muted">Личный кабинет</span>
        </div>
      </Link>
      <Button variant="ghost" onClick={() => void logout()}>
        Выйти
      </Button>
    </div>
  );
};
