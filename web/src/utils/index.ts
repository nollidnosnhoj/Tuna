import queryString from 'query-string';

export * from './axios'
export * from './format'

export const validationMessages = {
  required: function (field: string) {
    return `${field} is required.`;
  },
  min: function (field: string, min: number) {
    return `${field} must be at least ${min} characters long.`;
  },
  max: function (field: string, max: number) {
    return `${field} must be no more than ${max} characters long.`;
  },
};

export function isServer() {
  return typeof window === 'undefined'
}

export function generatePaginatedKey(key: string, page: number, size: number, params?: Record<string, any>) {
  const queryParams = { page, size, ...params };
  return key + queryString.stringify(queryParams);
}