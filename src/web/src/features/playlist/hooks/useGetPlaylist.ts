import {
  QueryKey,
  useQuery,
  UseQueryOptions,
  UseQueryResult,
} from "react-query";
import { getPlaylistRequest } from "../api";
import { Playlist } from "../api/types";

export const GET_PLAYLIST_KEY = (id: string): QueryKey => ["playlist", id];

export function useGetPlaylist(
  id: string,
  options: UseQueryOptions<Playlist> = {}
): UseQueryResult<Playlist> {
  return useQuery<Playlist>(
    GET_PLAYLIST_KEY(id),
    () => getPlaylistRequest(id),
    options
  );
}
