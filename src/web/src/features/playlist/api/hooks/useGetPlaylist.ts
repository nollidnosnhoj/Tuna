import { useCallback } from "react";
import {
  QueryKey,
  useQuery,
  UseQueryOptions,
  UseQueryResult,
} from "react-query";
import request from "~/lib/http";
import { ID } from "~/lib/types";
import { Playlist } from "../types";

export const GET_PLAYLIST_KEY = (playlistId: ID): QueryKey => [
  "playlist",
  playlistId,
];

export function useGetPlaylist(
  playlistId: ID,
  options: UseQueryOptions<Playlist> = {}
): UseQueryResult<Playlist> {
  const fetcher = useCallback(async () => {
    const { data } = await request<Playlist>({
      method: "GET",
      url: `playlists/${playlistId}`,
    });
    return data;
  }, [playlistId]);
  return useQuery<Playlist>(GET_PLAYLIST_KEY(playlistId), fetcher, options);
}
