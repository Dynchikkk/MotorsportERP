import { apiClient } from './client';
import type {
  PagedResponse,
  TournamentApplicationResponse,
  TournamentApplyRequest,
  TournamentCreateRequest,
  TournamentDetailsResponse,
  TournamentListQuery,
  TournamentReferenceDataResponse,
  TournamentResponse,
  TournamentResultRequest,
  TournamentUpdateRequest,
} from '@/shared/types/api';

export const tournamentsApi = {
  async getReferenceData() {
    const { data } = await apiClient.get<TournamentReferenceDataResponse>('/tournaments/referenceData');
    return data;
  },

  async getAll(query: TournamentListQuery = {}, page = 0, pageSize = 24) {
    const { data } = await apiClient.get<PagedResponse<TournamentResponse>>('/tournaments', {
      params: { ...query, page, pageSize },
    });
    return data;
  },

  async getById(tournamentId: string) {
    const { data } = await apiClient.get<TournamentDetailsResponse>(`/tournaments/${tournamentId}`);
    return data;
  },

  async getApplications(tournamentId: string) {
    const { data } = await apiClient.get<TournamentApplicationResponse[]>(
      `/tournaments/${tournamentId}/applications`,
    );
    return data;
  },

  async create(request: TournamentCreateRequest) {
    const { data } = await apiClient.post<string>('/tournaments', request);
    return data;
  },

  async update(tournamentId: string, request: TournamentUpdateRequest) {
    await apiClient.put(`/tournaments/${tournamentId}`, request);
  },

  async remove(tournamentId: string) {
    await apiClient.delete(`/tournaments/${tournamentId}`);
  },

  async addOrganizer(tournamentId: string, userId: string) {
    await apiClient.post(`/tournaments/${tournamentId}/organizers/${userId}`);
  },

  async start(tournamentId: string) {
    await apiClient.post(`/tournaments/${tournamentId}/start`);
  },

  async finish(tournamentId: string) {
    await apiClient.post(`/tournaments/${tournamentId}/finish`);
  },

  async cancel(tournamentId: string) {
    await apiClient.post(`/tournaments/${tournamentId}/cancel`);
  },

  async addResult(tournamentId: string, request: TournamentResultRequest) {
    await apiClient.post(`/tournaments/${tournamentId}/results`, request);
  },

  async apply(tournamentId: string, request: TournamentApplyRequest) {
    await apiClient.post(`/tournaments/${tournamentId}/apply`, request);
  },

  async approveApplication(applicationId: string) {
    await apiClient.post(`/applications/${applicationId}/approve`);
  },

  async rejectApplication(applicationId: string) {
    await apiClient.post(`/applications/${applicationId}/reject`);
  },

  async cancelApplication(applicationId: string) {
    await apiClient.post(`/applications/${applicationId}/cancel`);
  },

  async addPhoto(tournamentId: string, photoId: string) {
    await apiClient.post(`/tournaments/${tournamentId}/photos/${photoId}`);
  },

  async removePhoto(tournamentId: string, photoId: string) {
    await apiClient.delete(`/tournaments/${tournamentId}/photos/${photoId}`);
  },
};
