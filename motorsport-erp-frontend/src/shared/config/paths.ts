export const PATHS = {
  HOME: '/',
  
  AUTH: {
    LOGIN: '/login',
    REGISTER: '/register',
  },
  
  USER: {
    PROFILE: '/profile',
    PUBLIC_PROFILE: (id: string | number = ':id') => `/users/${id}`,
    MANAGEMENT: '/users',
    GARAGE: '/garage',
  },

  TOURNAMENTS: {
    ROOT: '/tournaments',
    CREATE: '/tournaments/create',
    DETAILS: (id: string | number = ':id') => `/tournaments/${id}`,
    EDIT: (id: string | number = ':id') => `/tournaments/${id}/edit`,
  },

  TRACKS: {
    ROOT: '/tracks',
    CREATE: '/tracks/create',
    DETAILS: (id: string | number = ':id') => `/tracks/${id}`,
  },
} as const;