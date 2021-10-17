import {
  useInfinitePagination,
  UseInfinitePaginationOptions,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import request from "~/lib/http";
import { OffsetPagedList, Playlist } from "~/lib/types";

export const GET_YOUR_PLAYLISTS_KEY = "your_playlists";

export function useYourPlaylists(
  options: UseInfinitePaginationOptions<Playlist> = {}
): UseInfinitePaginationReturnType<Playlist> {
  async function fetcher(offset = 0): Promise<OffsetPagedList<Playlist>> {
    const { data } = await request<OffsetPagedList<Playlist>>({
      method: "get",
      url: "me/playlists",
      params: {
        offset,
      },
    });
    return data;
  }

  return useInfinitePagination(GET_YOUR_PLAYLISTS_KEY, fetcher, options);
}
