import request from './request';

export default async function fetch<T = any>(url: string) {
  const { data } = await request<T>(url, {
    method: 'get'
  });

  return data;
}
