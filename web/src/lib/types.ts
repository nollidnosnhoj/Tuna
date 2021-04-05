export type MetaAuthor = {
  id: number;
  username: string;
  displayName: string;
}

export type ErrorResponse = {
  code: number;
  message: string;
  errors?: { [key: string]: string[] }
}

export type PagedList<T> = {
  items: T[],
  count: number;
  page: number;
  size: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export type CursorPagedList<T> = {
  items: T[],
  previous?: number;
  next?: number;
}

export interface PaginatedOptions {
  size?: number;
  params?: Record<string, any>
}