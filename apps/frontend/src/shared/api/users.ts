import { apiClient } from './client';
import type {
  PagedResponse,
  PublicUserProfileResponse,
  UserAdminResponse,
  UserProfileResponse,
  UserReferenceDataResponse,
  UserResponse,
  UserRole,
  UserUpdateRequest,
} from '@/shared/types/api';

export const usersApi = {
  async getById(id: string) {
    const { data } = await apiClient.get<UserResponse>(`/users/${id}`);
    return data;
  },

  async getPublicProfile(id: string) {
    const { data } = await apiClient.get<PublicUserProfileResponse>(`/users/${id}/profile`);
    return data;
  },

  async getMyProfile() {
    const { data } = await apiClient.get<UserProfileResponse>('/users/profile');
    return data;
  },

  async getUsers(search = '', page = 0, pageSize = 20) {
    const { data } = await apiClient.get<PagedResponse<UserResponse>>('/users', {
      params: { search: search || undefined, page, pageSize },
    });
    return data;
  },

  async getAdminUsers(page = 0, pageSize = 20) {
    const { data } = await apiClient.get<PagedResponse<UserAdminResponse>>('/users/admin', {
      params: { page, pageSize },
    });
    return data;
  },

  async updateProfile(request: UserUpdateRequest) {
    await apiClient.put('/users/profile', request);
  },

  async assignRole(userId: string, role: UserRole) {
    await apiClient.post(`/users/${userId}/assign-role`, role, {
      headers: { 'Content-Type': 'application/json' },
    });
  },

  async setBlockStatus(userId: string, isBlocked: boolean) {
    await apiClient.post(`/users/${userId}/block`, null, {
      params: { isBlocked },
    });
  },

  async getReferenceData() {
    const { data } = await apiClient.get<UserReferenceDataResponse>('/users/referenceData');
    return data;
  },
};
