import { apiClient } from './client';
import type { MediaFileResponse } from '@/shared/types/api';

export const filesApi = {
  async upload(file: File) {
    const formData = new FormData();
    formData.append('file', file);

    const { data } = await apiClient.post<MediaFileResponse>('/files/upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });

    return data;
  },

  async remove(fileId: string) {
    await apiClient.delete(`/files/${fileId}`);
  },
};
