import { useCallback } from "react";
import { useQuery, UseQueryOptions, UseQueryResult } from "react-query";
import request from "~/lib/http";
import { ID, Playlist } from "~/lib/types";
import { GET_PLAYLIST_KEY } from "~/lib/hooks/api/keys";

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
