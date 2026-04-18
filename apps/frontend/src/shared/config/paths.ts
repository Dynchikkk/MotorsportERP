export const PATHS = {
  HOME: '/',
  AUTH: {
    LOGIN: '/login',
    REGISTER: '/register',
  },
  USERS: {
    ROOT: '/users',
    PROFILE: '/profile',
    GARAGE: '/garage',
    DETAILS: (id = ':id') => `/users/${id}`,
  },
  TOURNAMENTS: {
    ROOT: '/tournaments',
    DETAILS: (id = ':id') => `/tournaments/${id}`,
  },
  TRACKS: {
    ROOT: '/tracks',
    DETAILS: (id = ':id') => `/tracks/${id}`,
  },
} as const;
