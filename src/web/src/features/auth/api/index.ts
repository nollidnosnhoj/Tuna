import Axios from "axios";
import SETTINGS from "~/lib/config";
import request from "~/lib/http";
import { setAccessTokenExpiration } from "~/lib/http/utils";
import { OffsetPagedList } from "~/lib/types";
import { AudioView } from "../../audio/api/types";
import { Playlist } from "../../playlist/api/types";
import { LoginFormValues } from "../components/Forms/Login";

export async function getCurrentUserAudiosRequest(
  offset = 0
): Promise<OffsetPagedList<AudioView>> {
  const { data } = await request<OffsetPagedList<AudioView>>({
    method: "get",
    url: "me/audios",
    params: {
      offset,
    },
  });
  return data;
}

export async function getCurrentUserPlaylistsRequest(
  offset = 0
): Promise<OffsetPagedList<Playlist>> {
  const { data } = await request<OffsetPagedList<Playlist>>({
    method: "get",
    url: "me/playlists",
    params: {
      offset,
    },
  });
  return data;
}

export async function getCurrentUserFavoriteAudiosRequest(
  offset = 0
): Promise<OffsetPagedList<AudioView>> {
  const { data } = await request<OffsetPagedList<AudioView>>({
    method: "get",
    url: "me/favorite/audios",
    params: { offset },
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
