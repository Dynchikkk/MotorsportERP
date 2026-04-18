import {
  CarClass,
  TournamentApplicationStatus,
  TournamentStatus,
  TrackStatus,
  UserRole,
  type MediaFileResponse,
} from '@/shared/types/api';
import { resolveMediaUrl } from '@/shared/lib/media';

const dateFormatter = new Intl.DateTimeFormat('ru-RU', {
  day: '2-digit',
  month: 'long',
  year: 'numeric',
});

const dateTimeFormatter = new Intl.DateTimeFormat('ru-RU', {
  day: '2-digit',
  month: 'short',
  year: 'numeric',
  hour: '2-digit',
  minute: '2-digit',
});

export const carClassLabels: Record<CarClass, string> = {
  [CarClass.Stock]: 'Stock',
  [CarClass.Street]: 'Street',
  [CarClass.Sport]: 'Sport',
  [CarClass.SuperSport]: 'Super Sport',
  [CarClass.Unlimited]: 'Unlimited',
};

export const trackStatusLabels: Record<TrackStatus, string> = {
  [TrackStatus.Unofficial]: 'Неофициальная',
  [TrackStatus.Confirmed]: 'Подтверждена',
  [TrackStatus.Official]: 'Официальная',
};

export const tournamentStatusLabels: Record<TournamentStatus, string> = {
  [TournamentStatus.RegistrationOpen]: 'Регистрация открыта',
  [TournamentStatus.Confirmed]: 'Подтверждён',
  [TournamentStatus.Active]: 'Идёт',
  [TournamentStatus.Finished]: 'Завершён',
  [TournamentStatus.Cancelled]: 'Отменён',
};

export const tournamentApplicationStatusLabels: Record<TournamentApplicationStatus, string> = {
  [TournamentApplicationStatus.Pending]: 'На рассмотрении',
  [TournamentApplicationStatus.Approved]: 'Подтверждена',
  [TournamentApplicationStatus.Rejected]: 'Отклонена',
  [TournamentApplicationStatus.Cancelled]: 'Отменена',
};

export const roleLabels: Record<UserRole, string> = {
  [UserRole.None]: 'Нет ролей',
  [UserRole.Racer]: 'Гонщик',
  [UserRole.Organizer]: 'Организатор',
  [UserRole.Moderator]: 'Модератор',
  [UserRole.SuperAdmin]: 'Суперпользователь',
};

export const formatDate = (value: string) => dateFormatter.format(new Date(value));
export const formatDateTime = (value: string) => dateTimeFormatter.format(new Date(value));
export const formatDateRange = (startDate: string, endDate: string) =>
  `${formatDate(startDate)} - ${formatDate(endDate)}`;

export const formatCarName = (brand: string, model: string, year?: number) =>
  [brand, model, year].filter(Boolean).join(' ');

export const formatLapTime = (value?: string | null) => {
  if (!value) {
    return 'Не указан';
  }

  const normalized = value.includes('.') ? value.split('.')[0] : value;
  return normalized;
};

export const getInitials = (name: string) =>
  name
    .split(' ')
    .map((item) => item[0])
    .join('')
    .slice(0, 2)
    .toUpperCase();

export const getAvatarUrl = (avatar?: MediaFileResponse | null) => resolveMediaUrl(avatar?.url ?? null);

export const getRoleNames = (roles: UserRole) => {
  const orderedRoles = [
    UserRole.SuperAdmin,
    UserRole.Moderator,
    UserRole.Organizer,
    UserRole.Racer,
  ];

  return orderedRoles.filter((role) => role !== UserRole.None && (roles & role) === role);
};
