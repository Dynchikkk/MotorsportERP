import { PATHS } from './paths';

export const primaryNavigation = [
  { to: PATHS.HOME, label: 'Обзор', icon: '01' },
  { to: PATHS.TOURNAMENTS.ROOT, label: 'Турниры', icon: '02' },
  { to: PATHS.TRACKS.ROOT, label: 'Трассы', icon: '03' },
  { to: PATHS.USERS.ROOT, label: 'Пилоты', icon: '04' },
] as const;

export const personalNavigation = [
  { to: PATHS.USERS.PROFILE, label: 'Мой профиль', icon: '05' },
  { to: PATHS.USERS.GARAGE, label: 'Гараж', icon: '06' },
] as const;
