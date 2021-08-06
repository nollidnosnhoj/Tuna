import { Playlist } from "~/features/playlist/types";
import {
  useInfinitePagination,
  UseInfinitePaginationOptions,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import { getCurrentUserPlaylistsRequest } from "../api";

export const GET_YOUR_PLAYLISTS_KEY = "your_playlists";

export function useYourPlaylists(
  options: UseInfinitePaginationOptions<Playlist> = {}
): UseInfinitePaginationReturnType<Playlist> {
  return useInfinitePagination(
    GET_YOUR_PLAYLISTS_KEY,
    (page) => getCurrentUserPlaylistsRequest(page),
    options
  );
}
