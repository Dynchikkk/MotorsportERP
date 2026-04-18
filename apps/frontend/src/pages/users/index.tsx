import { useDeferredValue, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '@/app/providers/AuthProvider';
import { UserBadge } from '@/entities/user/ui/UserBadge';
import { AssignRoleAction } from '@/features/profile/assign-role/AssignRoleAction';
import { BlockUserAction } from '@/features/profile/block-user/BlockUserAction';
import { usersApi } from '@/shared/api/users';
import { PATHS } from '@/shared/config/paths';
import { useAsyncData } from '@/shared/lib/hooks/useAsyncData';
import { hasRole, isModeratorOrAbove } from '@/shared/lib/roles';
import { roleLabels } from '@/shared/lib/format';
import { UserRole } from '@/shared/types/api';
import { Card } from '@/shared/ui/Card';
import { EmptyState } from '@/shared/ui/EmptyState';
import { InputField } from '@/shared/ui/Field';
import { Loader } from '@/shared/ui/Loader';
import { Notice } from '@/shared/ui/Notice';
import { PageHeader } from '@/shared/ui/PageHeader';
import { Pill } from '@/shared/ui/Pill';
import { PublicUserPage } from './PublicUserPage';

export { PublicUserPage };

export const UsersPage = () => {
  const { currentUser } = useAuth();
  const [search, setSearch] = useState('');
  const deferredSearch = useDeferredValue(search);
  const canModerate = isModeratorOrAbove(currentUser?.roles);
  const canAssignRoles = hasRole(currentUser?.roles, UserRole.SuperAdmin);

  const users = useAsyncData(() => usersApi.getUsers(deferredSearch, 0, 30), [deferredSearch]);
  const adminUsers = useAsyncData(
    () => usersApi.getAdminUsers(0, 30),
    [currentUser?.id],
    {
      immediate: canModerate,
      initialData: null,
    },
  );

  const roleValues = useMemo(
    () =>
      [UserRole.Racer, UserRole.Organizer, UserRole.Moderator, UserRole.SuperAdmin].filter((role) =>
        role !== UserRole.None,
      ),
    [],
  );

  if (users.isLoading && !users.data) {
    return <Loader label="Загружаем пользователей" fullHeight />;
  }

  if (users.error || !users.data) {
    return <Notice tone="error" message={users.error ?? 'Не удалось загрузить список пользователей'} />;
  }

  const reloadAdmin = async () => {
    if (canModerate) {
      await adminUsers.reload();
    }
  };

  return (
    <div className="page">
      <PageHeader
        eyebrow="Сообщество"
        title="Пилоты и профили"
        description="Публичный каталог пользователей и административный раздел для модерации и назначения ролей."
      />

      <Card className="page__section">
        <InputField
          label="Поиск по никнейму"
          value={search}
          onChange={(event) => setSearch(event.target.value)}
          placeholder="Например, DriftFox"
        />
      </Card>

      {users.data.items.length ? (
        <div className="cards-grid">
          {users.data.items.map((user) => (
            <Card key={user.id} className="entity-card">
              <UserBadge user={user} />
              <p className="muted">{user.bio || 'Пользователь пока не добавил описание.'}</p>
              <Link to={PATHS.USERS.DETAILS(user.id)}>Открыть профиль</Link>
            </Card>
          ))}
        </div>
      ) : (
        <EmptyState title="Никого не нашли" description="Попробуйте изменить поисковую строку." />
      )}

      {canModerate ? (
        <Card className="page__section">
          <PageHeader
            eyebrow="Администрирование"
            title="Панель управления пользователями"
            description="Модератор может блокировать аккаунты, а суперпользователь дополнительно назначает роли."
          />
          {adminUsers.isLoading && !adminUsers.data ? <Loader label="Загружаем административный список" /> : null}
          {adminUsers.error ? <Notice tone="error" message={adminUsers.error} /> : null}
          {adminUsers.data ? (
            <div style={{ overflowX: 'auto' }}>
              <table className="table">
                <thead>
                  <tr>
                    <th>Пользователь</th>
                    <th>Статус</th>
                    <th>Роли</th>
                    <th>Действия</th>
                  </tr>
                </thead>
                <tbody>
                  {adminUsers.data.items.map((user) => (
                    <tr key={user.id}>
                      <td>
                        <UserBadge user={user} />
                      </td>
                      <td>
                        <Pill tone={user.isBlocked ? 'danger' : 'success'}>
                          {user.isBlocked ? 'Заблокирован' : 'Активен'}
                        </Pill>
                      </td>
                      <td>
                        <div className="cluster">
                          {roleValues
                            .filter((role) => hasRole(user.roles, role))
                            .map((role) => (
                              <Pill key={role} tone="muted">
                                {roleLabels[role]}
                              </Pill>
                            ))}
                        </div>
                      </td>
                      <td>
                        <div className="stack">
                          <BlockUserAction userId={user.id} isBlocked={user.isBlocked} onChanged={reloadAdmin} />
                          {canAssignRoles ? <AssignRoleAction userId={user.id} onAssigned={reloadAdmin} /> : null}
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : null}
        </Card>
      ) : null}
    </div>
  );
};
