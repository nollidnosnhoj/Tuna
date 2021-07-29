import axios, { AxiosInstance } from "axios";
import { AxiosAuthRefreshRequestConfig } from "axios-auth-refresh";
import { IncomingMessage, ServerResponse } from "http";
import { NextApiRequest, NextApiResponse } from "next";
import { destroyCookie, parseCookies, setCookie } from "nookies";
import { useTokenStore } from "../stores";
import { ApiRequestConfig, TOKEN_CONSTANTS } from "./types";
import SETTINGS from "~/lib/config";
import { stringifyQueryObject } from "~/utils";

/**
 * Get the access token from the local storage.
 * @returns Get the access token from the local storage.
 */
export function getAccessTokenExpirationFromLocalStorage(): number {
  if (typeof window !== "undefined") {
    const a = window.localStorage.getItem(TOKEN_CONSTANTS.ACCESS_TOKEN_EXPIRES);
    if (!a) return 0;
    const parsed = parseInt(a);
    return isNaN(parsed) ? 0 : parsed;
  }
  return 0;
}

type RequestContext = NextApiRequest | IncomingMessage;
/**
 * Get the access token.
 * @param req The context containing the request. If the context is falsy, get the token from the document.
 * @returns Access token from the context.
 */
export function getAccessToken(req?: RequestContext): string {
  const { getState } = useTokenStore;
  return (
    getState().accessToken ||
    parseCookies({ req })[TOKEN_CONSTANTS.ACCESS_TOKEN]
  );
}

type ResponseContext = NextApiResponse | ServerResponse;
/**
 * Set the new access token.
 * @param token The new access token.
 * @param res The context containing the response. If the context is falsy, it will set in the document.
 */
export function setAccessToken(token: string, res?: ResponseContext): void {
  const { setState } = useTokenStore;
  setState({ accessToken: token });
  setCookie({ res }, TOKEN_CONSTANTS.ACCESS_TOKEN, token, {
    path: "/",
    sameSite: true,
    maxAge: 60 * 60 * 24 * 7,
  });
}

export function removeAccessToken(res?: ResponseContext): void {
  const { setState } = useTokenStore;
  setState({ accessToken: "", accessTokenExpires: 0 });
  destroyCookie({ res }, TOKEN_CONSTANTS.ACCESS_TOKEN, {
    path: "/",
    sameSite: true,
  });
}

/**
 * Get the access token expiration from the state (or local storage).
 * @returns The access token expiration in unix epoch.
 */
export function getAccessTokenExpiration(): number {
  const { getState } = useTokenStore;
  return (
    getState().accessTokenExpires || getAccessTokenExpirationFromLocalStorage()
  );
}

/**
 * Set the access token expiration date in state (if called in browser).
 * @param expires The expiration date of access token in unix epoch.
 */
export function setAccessTokenExpiration(expires: number): void {
  const { setState } = useTokenStore;
  setState({ accessTokenExpires: expires });
  if (typeof window !== "undefined") {
    localStorage.setItem(TOKEN_CONSTANTS.ACCESS_TOKEN_EXPIRES, expires + "");
  }
}

/**
 * Get the refresh token from given context.
 * @param req Context containing the request.
 * @returns The refresh token from the given context.
 */
export function getRefreshToken(req: RequestContext): string {
  return parseCookies({ req })[TOKEN_CONSTANTS.REFRESH_TOKEN];
}

/**
 * Set the new refresh token.
 * @param value The value of the new refresh token.
 * @param expires The expiration date in unix epoch.
 * @param res The context containing the response.
 */
export function setRefreshToken(
  value: string,
  expires: number,
  res: ResponseContext
): void {
  setCookie({ res }, TOKEN_CONSTANTS.REFRESH_TOKEN, value, {
    expires: new Date(expires * 1000),
    path: "/",
    httpOnly: true,
    sameSite: true,
  });
}

export function removeRefreshToken(res?: ResponseContext): void {
  destroyCookie({ res }, TOKEN_CONSTANTS.REFRESH_TOKEN, {
    path: "/",
    httpOnly: true,
    sameSite: true,
  });
}

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
  const accessToken = getAccessToken(req);
  return {
    ...axiosConfig,
    headers: {
      ...(!!axiosConfig.data && { "Content-Type": "application/json" }),
      ...(!!accessToken && { Authorization: `Bearer ${accessToken}` }),
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
