import { GetServerSidePropsContext } from "next";
import request from "~/lib/http";
import { IdSlug, PagedList } from "~/lib/types";
import { AudioId, AudioView } from "../../audio/api/types";
import {
  CreatePlaylistRequest,
  Playlist,
  PlaylistAudioId,
  PlaylistId,
  PlaylistRequest,
} from "./types";

export async function getPlaylistRequest(
  id: IdSlug,
  ctx?: GetServerSidePropsContext
): Promise<Playlist> {
  const { res, req } = ctx ?? {};
  const { data } = await request<Playlist>({
    method: "GET",
    url: `playlists/${id}`,
    req,
    res,
  });
  return data;
}

export async function getPlaylistAudiosRequest(
  id: PlaylistId,
  page = 1
): Promise<PagedList<AudioView>> {
  const { data } = await request<PagedList<AudioView>>({
    method: "GET",
    url: `playlists/${id}/audios`,
    params: {
      page,
    },
  });
  return data;
}

export async function createPlaylistRequest(
  inputs: CreatePlaylistRequest
): Promise<Playlist> {
  const { data } = await request<Playlist>({
    method: "post",
    url: "playlists",
    data: inputs,
  });
  return data;
}

export async function updatePlaylistDetailsRequest(
  id: PlaylistId,
  inputs: PlaylistRequest
): Promise<Playlist> {
  const { data } = await request({
    method: "put",
    url: `playlists/${id}`,
    data: inputs,
  });
  return data;
}

export async function removePlaylistRequest(id: string): Promise<void> {
  await request({
    method: "delete",
    url: `playlists/${id}`,
  });
}

export async function addAudiosToPlaylistRequest(
  id: number,
  audioIds: AudioId[]
): Promise<void> {
  await request({
    method: "put",
    url: `playlists/${id}/audios`,
    data: {
      audioIds,
    },
  });
}

export async function removeAudiosFromPlaylistRequests(
  id: PlaylistId,
  playlistAudioIds: PlaylistAudioId[]
): Promise<void> {
  await request({
    method: "delete",
    url: `playlists/${id}/audios`,
    data: {
      playlistAudioIds,
    },
  });
}

export async function checkDuplicatedAudiosRequest(
  id: number,
  audioIds: AudioId[]
): Promise<AudioId[]> {
  const { data } = await request<AudioId[]>({
    method: "post",
    url: `playlists/${id}/audios/duplicate`,
    data: {
      audioIds,
    },
  });
  return data;
}

export async function checkIfPlaylistFavoritedRequest(
  playlistId: string
): Promise<boolean> {
  try {
    const res = await request({
      method: "head",
      url: `me/favorites/playlists/${playlistId}`,
      validateStatus: (status) => {
        return status === 404 || status < 400;
      },
    });

    return res.status !== 404;
  } catch (err) {
    return false;
  }
}

export async function favoriteAPlaylistRequest(
  playlistId: string
): Promise<boolean> {
  await request({
    method: "PUT",
    url: `me/favorites/playlists/${playlistId}`,
  });
  return true;
}

export async function unfavoriteAPlaylistRequest(
  playlistId: string
): Promise<boolean> {
  await request({
    method: "DELETE",
    url: `me/favorites/playlists/${playlistId}`,
  });
  return true;
}
