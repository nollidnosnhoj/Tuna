/* eslint-disable @typescript-eslint/no-explicit-any */
import axios, { AxiosResponse, Method } from "axios";
import createAuthRefreshInterceptor, {
  AxiosAuthRefreshRequestConfig,
} from "axios-auth-refresh";
import { stringifyUrl } from "query-string";
import { refreshAccessToken } from "~/features/auth/services";
import config from "~/lib/config";
import { PagedList } from "~/lib/types";
import { isAxiosError, getAccessToken } from "../utils";

const backendServerAxios = axios.create({
  baseURL: config.BACKEND_API,
  withCredentials: true,
});

export function getBearer(token: string): string {
  return token ? `Bearer ${token}` : "";
}

createAuthRefreshInterceptor(backendServerAxios, async (failedRequest) => {
  return refreshAccessToken()
    .then(([newToken]) => {
      if (
        failedRequest &&
        isAxiosError(failedRequest) &&
        failedRequest.response
      ) {
        failedRequest.response.config.headers["Authorization"] = getBearer(
          newToken
        );
      }
      backendServerAxios.defaults.headers["Authorization"] = getBearer(
        newToken
      );
      return Promise.resolve();
    })
    .catch((err) => Promise.reject(err));
});

backendServerAxios.interceptors.request.use((request) => {
  if (!request.headers.Authorization) {
    const accessToken = getAccessToken();
    request.headers.Authorization = getBearer(accessToken);
  }
  return request;
});

interface RequestConfiguration {
  accessToken?: string;
  contentType?: string;
  skipAuthRefresh?: boolean;
}

function constructRequestConfiguration(
  config: RequestConfiguration = {}
): AxiosAuthRefreshRequestConfig {
  const {
    contentType = "application/json",
    accessToken,
    skipAuthRefresh = false,
  } = config;

  const headers = {
    "Content-Type": contentType,
  };

  if (accessToken) {
    Object.assign(headers, { Authorization: getBearer(accessToken) });
  }

  return {
    headers,
    skipAuthRefresh,
  };
}

function getRequest<TResponse = any>(
  route: string,
  params: Record<string, any> = {},
  config?: RequestConfiguration
): Promise<AxiosResponse<TResponse>> {
  const requestConfig = constructRequestConfiguration(config);
  return backendServerAxios.get<TResponse>(
    stringifyUrl({ url: route, query: params }),
    requestConfig
  );
}

function headRequest(
  route: string,
  config?: RequestConfiguration
): Promise<AxiosResponse<any>> {
  const requestConfig = constructRequestConfiguration(config);
  return backendServerAxios.head(route, requestConfig);
}

function postRequest<TResponse = any, TRequest = unknown>(
  route: string,
  body?: TRequest,
  config?: RequestConfiguration
): Promise<AxiosResponse<TResponse>> {
  const requestConfig = constructRequestConfiguration(config);
  return backendServerAxios.post<TResponse>(route, body, requestConfig);
}

function putRequest<TResponse = any, TRequest = unknown>(
  route: string,
  body?: TRequest,
  config?: RequestConfiguration
): Promise<AxiosResponse<TResponse>> {
  const requestConfig = constructRequestConfiguration(config);
  return backendServerAxios.put<TResponse>(route, body, requestConfig);
}

function patchRequest<TResponse = any, TRequest = unknown>(
  route: string,
  body?: TRequest,
  config?: RequestConfiguration
): Promise<AxiosResponse<TResponse>> {
  const requestConfig = constructRequestConfiguration(config);
  return backendServerAxios.patch<TResponse>(route, body, requestConfig);
}

function deleteRequest<TResponse = any>(
  route: string,
  config?: RequestConfiguration
): Promise<AxiosResponse<TResponse>> {
  const requestConfig = constructRequestConfiguration(config);
  return backendServerAxios.delete<TResponse>(route, requestConfig);
}

interface GenericRequestConfiguration<TRequest = any>
  extends RequestConfiguration {
  body?: TRequest;
  params?: Record<string, any>;
}

function request<TResponse = any, TRequest = unknown>(
  method: Method,
  route: string,
  config: GenericRequestConfiguration<TRequest> = {}
): Promise<AxiosResponse<TResponse>> {
  const { body, params, ...otherConfig } = config;
  const requestConfig = constructRequestConfiguration(otherConfig);
  return backendServerAxios.request<TResponse>({
    method: method,
    url: route,
    params,
    data: body,
    ...requestConfig,
  });
}

export interface FetchRequestOptions {
  accessToken?: string;
}

export function fetch<TResponse>(
  route: string,
  params: Record<string, any> = {},
  options: FetchRequestOptions = {}
): Promise<TResponse> {
  const { accessToken = getAccessToken() } = options;
  return new Promise<TResponse>((resolve, reject) => {
    getRequest(route, params, { accessToken })
      .then(({ data }) => {
        resolve(data);
      })
      .catch((err) => reject(err));
  });
}

export const fetchPages = async <TData>(
  key: string,
  params: Record<string, any> = {},
  page = 1,
  options: FetchRequestOptions = {}
): Promise<PagedList<TData>> => {
  const { accessToken = getAccessToken() } = options;
  return new Promise<PagedList<TData>>((resolve, reject) => {
    getRequest(key, { ...params, page }, { accessToken })
      .then(({ data }) => resolve(data))
      .catch((err) => reject(err));
  });
};

export default {
  get: getRequest,
  post: postRequest,
  put: putRequest,
  patch: patchRequest,
  delete: deleteRequest,
  head: headRequest,
  request: request,
};
