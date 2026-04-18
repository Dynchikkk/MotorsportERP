import {
  createContext,
  startTransition,
  useCallback,
  useContext,
  useEffect,
  useRef,
  useState,
  type PropsWithChildren,
} from 'react';
import { authApi } from '@/shared/api/auth';
import { usersApi } from '@/shared/api/users';
import {
  clearStoredTokens,
  getStoredAccessToken,
  hasStoredSession,
  setStoredTokens,
} from '@/shared/lib/auth';
import type {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  UserProfileResponse,
} from '@/shared/types/api';

type AuthContextValue = {
  currentUser: UserProfileResponse | null;
  isAuthenticated: boolean;
  isBootstrapping: boolean;
  login: (request: LoginRequest) => Promise<void>;
  register: (request: RegisterRequest) => Promise<void>;
  logout: () => Promise<void>;
  refreshCurrentUser: () => Promise<UserProfileResponse | null>;
  setCurrentUser: (user: UserProfileResponse | null) => void;
};

const AuthContext = createContext<AuthContextValue | null>(null);

const applyTokens = (response: AuthResponse) => {
  setStoredTokens(response.accessToken, response.refreshToken);
};

export const AuthProvider = ({ children }: PropsWithChildren) => {
  const [currentUser, setCurrentUserState] = useState<UserProfileResponse | null>(null);
  const [isBootstrapping, setIsBootstrapping] = useState(true);
  const refreshInFlightRef = useRef<Promise<UserProfileResponse | null> | null>(null);

  const setCurrentUser = useCallback((user: UserProfileResponse | null) => {
    startTransition(() => {
      setCurrentUserState(user);
    });
  }, []);

  const resetSession = useCallback(() => {
    clearStoredTokens();
    setCurrentUser(null);
    setIsBootstrapping(false);
  }, [setCurrentUser]);

  const refreshCurrentUser = useCallback(async () => {
    if (!getStoredAccessToken()) {
      resetSession();
      return null;
    }

    if (refreshInFlightRef.current) {
      return refreshInFlightRef.current;
    }

    const refreshRequest = (async () => {
      try {
        const profile = await usersApi.getMyProfile();
        setCurrentUser(profile);
        return profile;
      } catch {
        resetSession();
        return null;
      } finally {
        refreshInFlightRef.current = null;
        setIsBootstrapping(false);
      }
    })();

    refreshInFlightRef.current = refreshRequest;
    return refreshRequest;
  }, [resetSession, setCurrentUser]);

  useEffect(() => {
    const handleUnauthorized = () => resetSession();

    window.addEventListener('unauthorized', handleUnauthorized);
    return () => window.removeEventListener('unauthorized', handleUnauthorized);
  }, [resetSession]);

  useEffect(() => {
    if (!hasStoredSession()) {
      setIsBootstrapping(false);
      return;
    }

    void refreshCurrentUser();
  }, [refreshCurrentUser]);

  const login = async (request: LoginRequest) => {
    const tokens = await authApi.login(request);
    applyTokens(tokens);
    await refreshCurrentUser();
  };

  const register = async (request: RegisterRequest) => {
    await authApi.register(request);
    const tokens = await authApi.login({
      email: request.email,
      password: request.password,
    });

    applyTokens(tokens);
    await refreshCurrentUser();
  };

  const logout = async () => {
    try {
      if (hasStoredSession()) {
        await authApi.logout();
      }
    } finally {
      resetSession();
    }
  };

  const value: AuthContextValue = {
    currentUser,
    isAuthenticated: Boolean(currentUser),
    isBootstrapping,
    login,
    register,
    logout,
    refreshCurrentUser,
    setCurrentUser,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

// eslint-disable-next-line react-refresh/only-export-components
export const useAuth = () => {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error('useAuth must be used inside AuthProvider');
  }

  return context;
};
