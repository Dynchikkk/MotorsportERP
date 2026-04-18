import axios from 'axios';
import { env } from '@/shared/config/env';
import {
  clearStoredTokens,
  getStoredAccessToken,
  getStoredRefreshToken,
  setStoredTokens,
} from '@/shared/lib/auth';
import type { AuthResponse } from '@/shared/types/api';

const triggerUnauthorized = () => {
  clearStoredTokens();
  window.dispatchEvent(new Event('unauthorized'));
};

export const apiClient = axios.create({
  baseURL: env.apiUrl,
});

apiClient.interceptors.request.use((config) => {
  const accessToken = getStoredAccessToken();

  if (accessToken) {
    config.headers = config.headers ?? {};
    config.headers.Authorization = `Bearer ${accessToken}`;
  }

  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config as typeof error.config & { _retry?: boolean };
    const refreshToken = getStoredRefreshToken();
    const accessToken = getStoredAccessToken();

    if (
      error.response?.status === 401 &&
      !originalRequest?._retry &&
      accessToken &&
      refreshToken &&
      !String(originalRequest?.url ?? '').includes('/auth/refresh')
    ) {
      originalRequest._retry = true;

      try {
        const { data } = await axios.post<AuthResponse>(`${env.apiUrl}/auth/refresh`, {
          accessToken,
          refreshToken,
        });

        setStoredTokens(data.accessToken, data.refreshToken);
        originalRequest.headers = originalRequest.headers ?? {};
        originalRequest.headers.Authorization = `Bearer ${data.accessToken}`;

        return apiClient(originalRequest);
      } catch (refreshError) {
        triggerUnauthorized();
        return Promise.reject(refreshError);
      }
    }

    if (error.response?.status === 401) {
      triggerUnauthorized();
    }

    return Promise.reject(error);
  },
);
