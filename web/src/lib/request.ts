import { AxiosAuthRefreshRequestConfig } from 'axios-auth-refresh';
import apiAxios, { getBearer } from './axios';

type MethodType = 'get' | 'delete' | 'head' | 'post' | 'put' | 'patch'

interface RequestOptions<TRequest = any> {
  method?: MethodType,
  body?: TRequest,
  accessToken?: string;
  skipAuthRefresh?: boolean;
}

export default function request<TResponse = any, TRequest = any>(url: string, options: RequestOptions<TRequest> = {}) {
  let { method = 'get', body, accessToken, skipAuthRefresh = false } = options;

  const requestConfig: AxiosAuthRefreshRequestConfig = {
    url: url,
    method: method,
    data: body,
    skipAuthRefresh: skipAuthRefresh
  }

  if (accessToken) {
    Object.assign(requestConfig, { 
      headers: { 
        Authorization: getBearer(accessToken) 
      }
    })
  }

  return apiAxios.request<TResponse>(requestConfig);
}