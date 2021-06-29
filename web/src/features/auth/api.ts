import Axios from "axios";
import SETTINGS from "~/lib/config";
import request from "~/lib/http";
import { setAccessTokenExpiration } from "~/lib/http/utils";
import { PagedList } from "~/lib/types";
import { AudioData } from "../audio/types";
import { LoginFormValues } from "./components/LoginForm";

export async function getAuthenticatedUserAudios(
  page = 1
): Promise<PagedList<AudioData>> {
  const { data } = await request<PagedList<AudioData>>({
    method: "get",
    url: "me/audios",
    params: {
      page,
    },
  });
  return data;
}

export async function getAuthenticatedUserFavoriteAudios(
  page = 1
): Promise<PagedList<AudioData>> {
  const { data } = await request<PagedList<AudioData>>({
    method: "get",
    url: "me/favorite/audios",
    params: { page },
  });
  return data;
}

export async function authenticateUser(
  request: LoginFormValues
): Promise<[string, number]> {
  const { data } = await Axios.request({
    method: "post",
    baseURL: SETTINGS.FRONTEND_URL,
    url: "api/auth/login",
    withCredentials: true,
    data: request,
  });
  setAccessTokenExpiration(data.accessTokenExpires);
  return [data.accessToken, data.accessTokenExpires];
}

export async function refreshAccessToken(): Promise<[string, number]> {
  try {
    const { data } = await Axios.request({
      method: "post",
      baseURL: SETTINGS.FRONTEND_URL,
      url: "api/auth/refresh",
      withCredentials: true,
    });
    setAccessTokenExpiration(data.accessTokenExpires);
    return [data.accessToken, data.accessTokenExpires];
  } catch (err) {
    setAccessTokenExpiration(0);
    throw err;
  }
}

export async function revokeRefreshToken(): Promise<void> {
  try {
    await Axios.request({
      method: "post",
      baseURL: SETTINGS.FRONTEND_URL,
      url: "api/auth/revoke",
      withCredentials: true,
      validateStatus: (s) => s < 500,
    });
  } finally {
    setAccessTokenExpiration(0);
  }
}
