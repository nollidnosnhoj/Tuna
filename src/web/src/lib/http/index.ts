/* eslint-disable @typescript-eslint/no-explicit-any */
import { AxiosResponse } from "axios";
import { ApiRequestConfig } from "./types";
import { createApiAxiosInstance, createApiAxiosRequestConfig } from "./utils";

const httpClient = createApiAxiosInstance();

export default async function request<TResponse = any>(
  config: ApiRequestConfig
): Promise<AxiosResponse<TResponse>> {
  const requestConfig = await createApiAxiosRequestConfig(config);
  return await httpClient.request(requestConfig);
}
