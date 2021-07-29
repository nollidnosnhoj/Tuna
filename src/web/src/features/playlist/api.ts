import { GetServerSidePropsContext } from "next";
import request from "~/lib/http";
import { CreatePlaylistRequest, Playlist, PlaylistRequest } from "./types";

export async function getPlaylistRequest(
  id: string,
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
  id: string,
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
  id: string,
  audioIds: string[]
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
  id: string,
  audioIds: string[]
): Promise<void> {
  await request({
    method: "delete",
    url: `playlists/${id}/audios`,
    data: {
      audioIds,
    },
  });
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
