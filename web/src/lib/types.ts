export type Creator = {
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

export interface PaginatedOptions {
  size?: number;
  params?: Record<string, any>
}

export type Genre = {
  id: number;
  name: string;
  slug: string;
  count?: number;
}