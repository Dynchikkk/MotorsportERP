import { UserRole } from '@/shared/types/api';

export const hasRole = (roles: UserRole | undefined, role: UserRole) =>
  Boolean(roles && (roles & role) === role);

export const isModeratorOrAbove = (roles: UserRole | undefined) =>
  hasRole(roles, UserRole.Moderator) || hasRole(roles, UserRole.SuperAdmin);

export const isOrganizerOrAbove = (roles: UserRole | undefined) =>
  hasRole(roles, UserRole.Organizer) || isModeratorOrAbove(roles);
