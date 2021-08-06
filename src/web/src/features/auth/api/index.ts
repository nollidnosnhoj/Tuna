import Axios from "axios";
import SETTINGS from "~/lib/config";
import request from "~/lib/http";
import { setAccessTokenExpiration } from "~/lib/http/utils";
import { PagedList } from "~/lib/types";
import { AudioView } from "../../audio/api/types";
import { Playlist } from "../../playlist/api/types";
import { LoginFormValues } from "../components/Forms/Login";

export async function getCurrentUserAudiosRequest(
  page = 1
): Promise<PagedList<AudioView>> {
  const { data } = await request<PagedList<AudioView>>({
    method: "get",
    url: "me/audios",
    params: {
      page,
    },
  });
  return data;
}

export async function getCurrentUserPlaylistsRequest(
  page = 1
): Promise<PagedList<Playlist>> {
  const { data } = await request<PagedList<Playlist>>({
    method: "get",
    url: "me/playlists",
    params: {
      page,
    },
  });
  return data;
}

export async function getCurrentUserFavoriteAudiosRequest(
  page = 1
): Promise<PagedList<AudioView>> {
  const { data } = await request<PagedList<AudioView>>({
    method: "get",
    url: "me/favorite/audios",
    params: { page },
  });
  return data;
}

export async function authenticateRequest(
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

export async function refreshAccessTokenRequest(): Promise<[string, number]> {
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

export async function revokeRefreshTokenRequest(): Promise<void> {
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
