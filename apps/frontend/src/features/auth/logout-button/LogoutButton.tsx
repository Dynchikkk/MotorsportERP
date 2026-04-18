import { useAuth } from '@/app/providers/AuthProvider';
import { Button } from '@/shared/ui/Button';

export const LogoutButton = () => {
  const { logout } = useAuth();

  return (
    <Button variant="ghost" onClick={() => void logout()}>
      Выйти
    </Button>
  );
};
