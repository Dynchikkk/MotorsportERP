import { Link } from 'react-router-dom';
import { UserMenu } from '@/widgets/user-menu/UserMenu';

type HeaderProps = {
  compact?: boolean;
};

export const Header = ({ compact = false }: HeaderProps) => {
  return (
    <header className={`app-header${compact ? ' app-header--compact' : ''}`}>
      <div className="app-header__inner">
        <Link className="brand" to="/">
          <span className="brand__mark">M</span>
          <span className="brand__meta">
            <span>Motorsport ERP</span>
            <span className="brand__eyebrow">Amateur event control</span>
          </span>
        </Link>
        <UserMenu />
      </div>
    </header>
  );
};
