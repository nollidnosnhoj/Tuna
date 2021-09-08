export type MetaAuthor = {
  id: number;
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
