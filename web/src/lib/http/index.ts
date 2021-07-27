/* eslint-disable @typescript-eslint/no-explicit-any */
import { AxiosResponse } from "axios";
import createAuthRefreshInterceptor from "axios-auth-refresh";
import { refreshAccessTokenRequest } from "~/features/auth/api";
import { isAxiosError } from "~/utils";
import { ApiRequestConfig } from "./types";
import {
  createApiAxiosInstance,
  createApiAxiosRequestConfig,
  getAccessToken,
} from "./utils";

const httpClient = createApiAxiosInstance();

createAuthRefreshInterceptor(
  httpClient,
  (err) =>
    refreshAccessTokenRequest()
      .then(([newToken]) => {
        if (err && isAxiosError(err) && err.response) {
          err.response.config.headers["Authorization"] = `Bearer ${newToken}`;
        }
        httpClient.defaults.headers["Authorization"] = `Bearer ${newToken}`;
        return Promise.resolve();
      })
      .catch((err) => Promise.reject(err)),
  {
    pauseInstanceWhileRefreshing: false,
  }
);

httpClient.interceptors.request.use((request) => {
  const accessToken = getAccessToken();
  if (!request.headers.Authorization && accessToken) {
    request.headers.Authorization = `Bearer ${accessToken}`;
  }
  return request;
});

export default async function request<TResponse = any>(
  config: ApiRequestConfig
): Promise<AxiosResponse<TResponse>> {
  const requestConfig = await createApiAxiosRequestConfig(config);
  return await httpClient.request(requestConfig);
}
