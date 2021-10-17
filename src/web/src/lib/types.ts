export type ID = string | number;

export type MetaAuthor = {
  id: ID;
  userName: string;
  picture?: string;
};

export type ErrorResponse = {
  code: number;
  message: string;
  errors?: { [key: string]: string[] };
};

export type PagedList<T> = {
  items: T[];
  count: number;
  page: number;
  size: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
};

export type OffsetPagedList<T> = {
  items: T[];
  next?: number;
  size: number;
};

export type CursorPagedList<T> = {
  items: T[];
  next?: number;
  size: number;
};

export interface PaginatedOptions {
  size?: number;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  params?: Record<string, any>;
}

export interface ImageUploadResponse {
  url: string;
}

export interface AudioView {
  id: ID;
  title: string;
  slug: string;
  description: string;
  tags: string[];
  duration: number;
  size: number;
  picture?: string;
  isFavorited?: boolean;
  created: string;
  lastModified?: string;
  audio: string;
  user: MetaAuthor;
}

export interface AudioRequest {
  title: string;
  description?: string;
  tags: string[];
}

export interface CreateAudioRequest extends AudioRequest {
  uploadId: string;
  fileName: string;
  fileSize: number;
  duration: number;
}

export interface Playlist {
  id: ID;
  title: string;
  description: string;
  picture?: string;
  tags: string[];
  audios: AudioView[];
  user: MetaAuthor;
}

export interface PlaylistAudio {
  id: number;
  audio: AudioView;
}

export interface PlaylistRequest {
  title: string;
  description?: string;
  tags: string[];
}

export interface CreatePlaylistRequest extends PlaylistRequest {
  audioIds: ID[];
}

export type CurrentUser = {
  id: ID;
  userName: string;
  email: string;
  role: string;
};
export type Profile = {
  id: ID;
  userName: string;
  picture: string;
};
