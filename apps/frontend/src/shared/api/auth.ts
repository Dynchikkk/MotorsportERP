import { apiClient } from './client';
import type {
  AuthResponse,
  LoginRequest,
  RefreshTokenRequest,
  RegisterRequest,
  UserResponse,
} from '@/shared/types/api';

export const authApi = {
  async register(request: RegisterRequest) {
    const { data } = await apiClient.post<UserResponse>('/auth/register', request);
    return data;
  },

  async login(request: LoginRequest) {
    const { data } = await apiClient.post<AuthResponse>('/auth/login', request);
    return data;
  },

  async refresh(request: RefreshTokenRequest) {
    const { data } = await apiClient.post<AuthResponse>('/auth/refresh', request);
    return data;
  },

  async logout() {
    await apiClient.post('/auth/logout');
  },
};
