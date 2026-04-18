import { env } from '@/shared/config/env';
import type { MediaFileResponse } from '@/shared/types/api';

const getApiOrigin = () => {
  const normalized = env.apiUrl.replace(/\/+$/, '');
  if (normalized.toLowerCase().endsWith('/api')) {
    return normalized.slice(0, -'/api'.length);
  }
  return normalized;
};

/** Backend stores paths like `/uploads/guid.jpg`; resolve against API host (not the SPA origin). */
export const resolveMediaUrl = (url: string | null | undefined): string | null => {
  if (!url) {
    return null;
  }
  if (/^https?:\/\//i.test(url)) {
    return url;
  }
  const path = url.startsWith('/') ? url : `/${url}`;
  return `${getApiOrigin()}${path}`;
};

type SyncMediaParams = {
  currentPhotos: MediaFileResponse[];
  nextPhotos: MediaFileResponse[];
  addPhoto: (photoId: string) => Promise<void>;
  removePhoto: (photoId: string) => Promise<void>;
};

export const syncMediaFiles = async ({
  currentPhotos,
  nextPhotos,
  addPhoto,
  removePhoto,
}: SyncMediaParams) => {
  const currentIds = new Set(currentPhotos.map((photo) => photo.id));
  const nextIds = new Set(nextPhotos.map((photo) => photo.id));

  const additions = nextPhotos.filter((photo) => !currentIds.has(photo.id));
  const removals = currentPhotos.filter((photo) => !nextIds.has(photo.id));

  await Promise.all(additions.map((photo) => addPhoto(photo.id)));
  await Promise.all(removals.map((photo) => removePhoto(photo.id)));
};
