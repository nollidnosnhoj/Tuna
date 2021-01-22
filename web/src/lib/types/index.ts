export type Creator = {
  id: number;
  username: string;
  displayName: string;
}

export type AuthResult = {
  accessToken: string,
}

export type ErrorResponse = {
  title: string;
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
