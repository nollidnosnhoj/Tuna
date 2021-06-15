/* eslint-disable @typescript-eslint/no-explicit-any */
import { AxiosResponse } from "axios";
import { ApiRequestConfig, NextSSRContext } from "./types";
import { createApiAxiosInstance, createApiAxiosRequestConfig } from "./utils";

export default async function request<TResponse = any>(
  config: ApiRequestConfig,
  ctx?: NextSSRContext
): Promise<AxiosResponse<TResponse>> {
  const httpClient = createApiAxiosInstance(ctx);
  const requestConfig = await createApiAxiosRequestConfig(config, ctx);
  return await httpClient.request(requestConfig);
}
