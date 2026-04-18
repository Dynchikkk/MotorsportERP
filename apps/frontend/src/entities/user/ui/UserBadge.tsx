import { Link } from 'react-router-dom';
import { PATHS } from '@/shared/config/paths';
import { getRoleNames, roleLabels } from '@/shared/lib/format';
import type { UserProfileResponse, UserResponse } from '@/shared/types/api';
import { Avatar } from '@/shared/ui/Avatar';
import { Pill } from '@/shared/ui/Pill';

type UserBadgeProps = {
  user: UserResponse | UserProfileResponse;
  showRoles?: boolean;
};

export const UserBadge = ({ user, showRoles = false }: UserBadgeProps) => {
  const roles = 'roles' in user ? getRoleNames(user.roles) : [];

  return (
    <div className="user-badge">
      <Avatar name={user.nickname} avatar={user.avatar} />
      <div className="stack" style={{ gap: 6 }}>
        <Link to={PATHS.USERS.DETAILS(user.id)}>
          <strong>{user.nickname}</strong>
        </Link>
        <span className="muted">{user.raceCount} проведённых гонок</span>
        {showRoles && roles.length > 0 ? (
          <div className="cluster">
            {roles.map((role) => (
              <Pill key={role} tone="accent">
                {roleLabels[role]}
              </Pill>
            ))}
          </div>
        ) : null}
      </div>
    </div>
  );
};
