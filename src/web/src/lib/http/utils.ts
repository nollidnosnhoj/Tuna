import axios, { AxiosInstance } from "axios";
import { AxiosAuthRefreshRequestConfig } from "axios-auth-refresh";
import { ApiRequestConfig } from "./types";
import SETTINGS from "~/lib/config";
import { stringifyQueryObject } from "~/utils";

/**
 * This configures the axios request config based on the input config.
 * @param config Configuration for the HTTP Request Config
 * @param ctx Context containing the request and response
 * @returns Axios Request Config
 */
export async function createApiAxiosRequestConfig(
  config: ApiRequestConfig
): Promise<AxiosAuthRefreshRequestConfig> {
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const { res, req, ...axiosConfig } = config;
  return {
    ...axiosConfig,
    headers: {
      ...(!!axiosConfig.data && { "Content-Type": "application/json" }),
    },
  };
}

/**
 * Create an axios instance that contains appropriate information for requesting data from the backend.
 * @returns Axios instance for the backend API
 */
export function createApiAxiosInstance(): AxiosInstance {
  return axios.create({
    baseURL: SETTINGS.BACKEND_API,
    withCredentials: true,
    paramsSerializer: (params) => stringifyQueryObject(params),
  });
}
