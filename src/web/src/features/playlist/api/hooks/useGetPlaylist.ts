import { useCallback } from "react";
import {
  QueryKey,
  useQuery,
  UseQueryOptions,
  UseQueryResult,
} from "react-query";
import request from "~/lib/http";
import { Playlist, PlaylistId } from "../types";

export const GET_PLAYLIST_KEY = (id: PlaylistId): QueryKey => ["playlist", id];

export function useGetPlaylist(
  id: number,
  options: UseQueryOptions<Playlist> = {}
): UseQueryResult<Playlist> {
  const fetcher = useCallback(async () => {
    const { data } = await request<Playlist>({
      method: "GET",
      url: `playlists/${id}`,
    });
    return data;
  }, [id]);
  return useQuery<Playlist>(GET_PLAYLIST_KEY(id), fetcher, options);
}
