export enum UserRole {
  None = 0,
  Racer = 1,
  Organizer = 2,
  Moderator = 4,
  SuperAdmin = 8,
}

export enum CarClass {
  Stock = 0,
  Street = 1,
  Sport = 2,
  SuperSport = 3,
  Unlimited = 4,
}

export enum TrackStatus {
  Unofficial = 0,
  Confirmed = 1,
  Official = 2,
}

export enum TournamentStatus {
  RegistrationOpen = 0,
  Confirmed = 1,
  Active = 2,
  Finished = 3,
  Cancelled = 4,
}

export enum TournamentApplicationStatus {
  Pending = 0,
  Approved = 1,
  Rejected = 2,
  Cancelled = 3,
}

export type Guid = string;

export type EnumValueResponse = {
  name: string;
  value: number;
};

export type MediaFileResponse = {
  id: Guid;
  url: string;
};

export type PagedResponse<T> = {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
};

export type AuthResponse = {
  accessToken: string;
  refreshToken: string;
};

export type LoginRequest = {
  email: string;
  password: string;
};

export type RegisterRequest = LoginRequest & {
  nickname: string;
};

export type RefreshTokenRequest = AuthResponse;

export type UserResponse = {
  id: Guid;
  nickname: string;
  bio?: string | null;
  raceCount: number;
  avatar?: MediaFileResponse | null;
};

export type UserTournamentEntryResponse = {
  applicationId: Guid;
  tournamentId: Guid;
  tournamentName: string;
  tournamentStatus: TournamentStatus;
  startDate: string;
  endDate: string;
  trackId: Guid;
  trackName: string;
  carId: Guid;
  carName: string;
  applicationStatus: TournamentApplicationStatus;
  position?: number | null;
  bestLapTime?: string | null;
};

export type PublicUserProfileResponse = UserResponse & {
  carsCount: number;
  tournamentsCount: number;
  cars: CarResponse[];
  currentTournaments: UserTournamentEntryResponse[];
  tournamentHistory: UserTournamentEntryResponse[];
};

export type UserProfileResponse = PublicUserProfileResponse & {
  email: string;
  roles: UserRole;
  isBlocked: boolean;
  applications: UserTournamentEntryResponse[];
  organizedTournaments: TournamentResponse[];
};

export type UserAdminResponse = {
  id: Guid;
  nickname: string;
  email: string;
  roles: UserRole;
  isBlocked: boolean;
  raceCount: number;
  avatar?: MediaFileResponse | null;
};

export type UserUpdateRequest = {
  nickname: string;
  bio?: string | null;
  avatar?: MediaFileResponse | null;
};

export type UserReferenceDataResponse = {
  userRoles: EnumValueResponse[];
};

export type CarResponse = {
  id: Guid;
  carClass: CarClass;
  brand: string;
  model: string;
  year: number;
  description?: string | null;
  photos: MediaFileResponse[];
};

export type CarCreateRequest = {
  carClass: CarClass;
  brand: string;
  model: string;
  year: number;
  description?: string | null;
};

export type CarUpdateRequest = CarCreateRequest;

export type CarReferenceDataResponse = {
  carClasses: EnumValueResponse[];
};

export type TrackResponse = {
  id: Guid;
  status: TrackStatus;
  name: string;
  location: string;
  photos: MediaFileResponse[];
  voteCount: number;
};

export type TrackDetailsResponse = TrackResponse & {
  confirmationThreshold: number;
  createdBy?: UserResponse | null;
  upcomingTournaments: TournamentResponse[];
  pastTournaments: TournamentResponse[];
};

export type TrackCreateRequest = {
  name: string;
  location: string;
  status?: TrackStatus | null;
};

export type TrackUpdateRequest = {
  name: string;
  location: string;
};

export type TrackListQuery = {
  search?: string;
  status?: TrackStatus;
};

export type TrackReferenceDataResponse = {
  minRacesToCreateTrack: number;
  defaultTrackConfirmationThreshold: number;
  trackStatuses: EnumValueResponse[];
};

export type TournamentResponse = {
  id: Guid;
  name: string;
  status: TournamentStatus;
  startDate: string;
  endDate: string;
  trackId: Guid;
  trackName: string;
  allowedCarClass: CarClass;
  participantsCount: number;
  applicationsCount: number;
  photos: MediaFileResponse[];
};

export type TournamentApplicationResponse = {
  id: Guid;
  status: TournamentApplicationStatus;
  user: UserResponse;
  car: CarResponse;
};

export type TournamentResultResponse = {
  id: Guid;
  position: number;
  bestLapTime?: string | null;
  user: UserResponse;
};

export type TournamentDetailsResponse = {
  id: Guid;
  name: string;
  status: TournamentStatus;
  startDate: string;
  endDate: string;
  trackId: Guid;
  description: string;
  allowedCarClass: CarClass;
  requiredRaceCount: number;
  participantsCount: number;
  applicationsCount: number;
  requiredParticipants: number;
  track?: TrackResponse | null;
  organizers: UserResponse[];
  participants: TournamentApplicationResponse[];
  results: TournamentResultResponse[];
  photos: MediaFileResponse[];
};

export type TournamentCreateRequest = {
  name: string;
  startDate: string;
  endDate: string;
  trackId: Guid;
  allowedCarClass: CarClass;
  description: string;
  requiredRaceCount: number;
  requiredParticipants: number;
};

export type TournamentUpdateRequest = {
  startDate: string;
  endDate: string;
  description: string;
  requiredParticipants: number;
};

export type TournamentApplyRequest = {
  tournamentId: Guid;
  carId: Guid;
};

export type TournamentResultRequest = {
  userId: Guid;
  position: number;
  bestLapTime?: string | null;
};

export type TournamentListQuery = {
  search?: string;
  status?: TournamentStatus;
  trackId?: Guid;
};

export type TournamentReferenceDataResponse = {
  minRacesToBecomeOrganizer: number;
  tournamentStatuses: EnumValueResponse[];
  tournamentApplicationStatuses: EnumValueResponse[];
};
