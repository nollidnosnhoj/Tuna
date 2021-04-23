import axios, { Method } from 'axios'
import createAuthRefreshInterceptor, { AxiosAuthRefreshRequestConfig } from 'axios-auth-refresh';
import queryString from 'query-string'
import config from '~/lib/config';
import { refreshAccessToken } from "~/features/auth/services";
import { isAxiosError } from './axios';
import { getAccessToken } from './cookies';
import { PagedList } from '~/lib/types';

const backendServerAxios = axios.create({
  baseURL: config.BACKEND_API,
  withCredentials: true
});

export function getBearer(token: string) {
  return token ? `Bearer ${token}` : ''
}

createAuthRefreshInterceptor(backendServerAxios, async (failedRequest) => {
  return refreshAccessToken().then((data) => {
    if (failedRequest && isAxiosError(failedRequest) && failedRequest.response) {
      failedRequest.response.config.headers['Authorization'] = getBearer(data.accessToken);
    }
    backendServerAxios.defaults.headers['Authorization'] = getBearer(data.accessToken);
    return Promise.resolve();
  }).catch(err => Promise.reject(err));
});

backendServerAxios.interceptors.request.use(request => {
  if (!request.headers.Authorization) {
    const accessToken = getAccessToken();
    request.headers.Authorization = getBearer(accessToken);
  }
  return request;
});

interface RequestConfiguration {
  accessToken?: string;
  skipAuthRefresh?: boolean;
}

function constructRouteWithQueryParams(route: string, params: Record<string, any> = {}) {
  if (Object.keys(params).length === 0) return route;
  return `${route}?${queryString.stringify(params)}`
}

function constructRequestConfiguration(config?: RequestConfiguration): AxiosAuthRefreshRequestConfig {
  function constructAuthorizationHeader(accessToken?: string) {
    if (!accessToken) return {};
    return { 'Authorization': getBearer(accessToken) };
  }

  if (!config) return {};

  const {
    accessToken,
    skipAuthRefresh = false
  } = config;

  return {
    headers: { ...constructAuthorizationHeader(accessToken) },
    skipAuthRefresh: skipAuthRefresh
  }
}

function getRequest<TResponse = any>(route: string, params: Record<string,any> = {}, config?: RequestConfiguration) {
  return backendServerAxios.get<TResponse>(constructRouteWithQueryParams(route, params), constructRequestConfiguration(config));
}

function headRequest(route: string, config?: RequestConfiguration) {
  return backendServerAxios.head(route, constructRequestConfiguration(config));
}

function postRequest<TResponse = any, TRequest = unknown>(route: string, body?: TRequest, config?: RequestConfiguration) {
  return backendServerAxios.post<TResponse>(route, body, constructRequestConfiguration(config));
}

function putRequest<TResponse = any, TRequest = unknown>(route: string, body?: TRequest, config?: RequestConfiguration) {
  return backendServerAxios.put<TResponse>(route, body, constructRequestConfiguration(config));
}

function patchRequest<TResponse = any, TRequest = unknown>(route: string, body?: TRequest, config?: RequestConfiguration) {
  return backendServerAxios.patch<TResponse>(route, body, constructRequestConfiguration(config));
}

function deleteRequest<TResponse = any>(route: string, config?: RequestConfiguration) {
  return backendServerAxios.delete<TResponse>(route, constructRequestConfiguration(config));
}

function request<TResponse = any, TRequest = unknown>(route: string, method: Method, body?: TRequest, config?: RequestConfiguration) {
  return backendServerAxios.request<TResponse>({
    method: method,
    url: route,
    data: body,
    ...constructRequestConfiguration(config)
  });
}

export interface FetchAudioOptions {
  accessToken?: string;
}

export function fetch<TResponse>(route: string, params: Record<string, any> = {}, options: FetchAudioOptions = {}) {
  const accessToken = options.accessToken || getAccessToken();
  return new Promise<TResponse>((resolve, reject) => {
    getRequest(route, params, {accessToken}).then(({ data }) => {
      resolve(data)
    }).catch(err => reject(err));
  })
}

export const fetchPages = async <TData>(key: string, params: Record<string, any> = {}, page: number = 1, options: FetchAudioOptions = {}) => {
  const { data } = await getRequest<PagedList<TData>>(key, { ...params, page }, {
    accessToken: options.accessToken
  });
  return data;
}

export default {
  get: getRequest,
  post: postRequest,
  put: putRequest,
  patch: patchRequest,
  delete: deleteRequest,
  head: headRequest,
  request: request
}