import { apiClient } from './client';
import type {
  PagedResponse,
  TrackCreateRequest,
  TrackDetailsResponse,
  TrackListQuery,
  TrackReferenceDataResponse,
  TrackResponse,
  TrackUpdateRequest,
} from '@/shared/types/api';

export const tracksApi = {
  async getReferenceData() {
    const { data } = await apiClient.get<TrackReferenceDataResponse>('/tracks/referenceData');
    return data;
  },

  async getAll(query: TrackListQuery = {}, page = 0, pageSize = 24) {
    const { data } = await apiClient.get<PagedResponse<TrackResponse>>('/tracks', {
      params: { ...query, page, pageSize },
    });
    return data;
  },

  async getById(trackId: string) {
    const { data } = await apiClient.get<TrackDetailsResponse>(`/tracks/${trackId}`);
    return data;
  },

  async create(request: TrackCreateRequest) {
    const { data } = await apiClient.post<string>('/tracks', request);
    return data;
  },

  async update(trackId: string, request: TrackUpdateRequest) {
    await apiClient.put(`/tracks/${trackId}`, request);
  },

  async remove(trackId: string) {
    await apiClient.delete(`/tracks/${trackId}`);
  },

  async vote(trackId: string, isPositive: boolean) {
    await apiClient.post(`/tracks/${trackId}/vote`, null, {
      params: { isPositive },
    });
  },

  async confirm(trackId: string) {
    await apiClient.post(`/tracks/${trackId}/confirm`);
  },

  async makeOfficial(trackId: string) {
    await apiClient.post(`/tracks/${trackId}/official`);
  },

  async addPhoto(trackId: string, photoId: string) {
    await apiClient.post(`/tracks/${trackId}/photos/${photoId}`);
  },

  async removePhoto(trackId: string, photoId: string) {
    await apiClient.delete(`/tracks/${trackId}/photos/${photoId}`);
  },
};
