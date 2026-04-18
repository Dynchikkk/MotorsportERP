import { apiClient } from './client';
import type {
  CarCreateRequest,
  CarReferenceDataResponse,
  CarResponse,
  CarUpdateRequest,
  PagedResponse,
} from '@/shared/types/api';

export const carsApi = {
  async getReferenceData() {
    const { data } = await apiClient.get<CarReferenceDataResponse>('/cars/referenceData');
    return data;
  },

  async getMyCars(page = 0, pageSize = 50) {
    const { data } = await apiClient.get<PagedResponse<CarResponse>>('/cars/my', {
      params: { page, pageSize },
    });
    return data;
  },

  async getUserCars(userId: string, page = 0, pageSize = 50) {
    const { data } = await apiClient.get<PagedResponse<CarResponse>>(`/cars/user/${userId}`, {
      params: { page, pageSize },
    });
    return data;
  },

  async create(request: CarCreateRequest) {
    const { data } = await apiClient.post<string>('/cars', request);
    return data;
  },

  async update(carId: string, request: CarUpdateRequest) {
    await apiClient.put(`/cars/${carId}`, request);
  },

  async remove(carId: string) {
    await apiClient.delete(`/cars/${carId}`);
  },

  async addPhoto(carId: string, photoId: string) {
    await apiClient.post(`/cars/${carId}/photos/${photoId}`);
  },

  async removePhoto(carId: string, photoId: string) {
    await apiClient.delete(`/cars/${carId}/photos/${photoId}`);
  },
};
