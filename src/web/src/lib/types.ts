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

export interface Audio {
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
  src: string;
  user: MetaAuthor;
}

export interface EditAudioRequest {
  title: string;
  description?: string;
  tags: string[];
}

export interface CreateAudioRequest extends EditAudioRequest {
  uploadId: string;
  fileName: string;
  fileSize: number;
  duration: number;
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
