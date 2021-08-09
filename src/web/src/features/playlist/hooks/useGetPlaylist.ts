import {
  QueryKey,
  useQuery,
  UseQueryOptions,
  UseQueryResult,
} from "react-query";
import { getPlaylistRequest } from "../api";
import { Playlist, PlaylistId } from "../api/types";

export const GET_PLAYLIST_KEY = (id: PlaylistId): QueryKey => ["playlist", id];

export function useGetPlaylist(
  id: PlaylistId,
  options: UseQueryOptions<Playlist> = {}
): UseQueryResult<Playlist> {
  return useQuery<Playlist>(
    GET_PLAYLIST_KEY(id),
    () => getPlaylistRequest(id),
    options
  );
}
