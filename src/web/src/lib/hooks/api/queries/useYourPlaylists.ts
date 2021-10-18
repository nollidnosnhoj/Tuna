import {
  useInfinitePagination,
  UseInfinitePaginationOptions,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import request from "~/lib/http";
import { OffsetPagedList, Playlist } from "~/lib/types";
import { GET_YOUR_PLAYLISTS_KEY } from "~/lib/hooks/api/keys";

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
