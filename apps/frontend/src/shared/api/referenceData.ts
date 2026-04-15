import { apiClient } from './client';

export interface EnumValue {
  value: number;
  name: string;
}

export interface ReferenceData {
  carClasses: EnumValue[];
  trackStatuses: EnumValue[];
  tournamentStatuses: EnumValue[];
  tournamentApplicationStatuses: EnumValue[];
  userRoles: EnumValue[];
  minRacesToCreateTrack: number;
  defaultTrackConfirmationThreshold: number;
  minRacesToBecomeOrganizer: number;
}

export const getReferenceData = async (): Promise<ReferenceData> => {
  const { data } = await apiClient.get<ReferenceData>('/reference-data');
  return data;
};