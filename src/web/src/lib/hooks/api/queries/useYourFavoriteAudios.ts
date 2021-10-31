import {
  useInfinitePagination,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import request from "~/lib/http";
import { Audio, OffsetPagedList } from "~/lib/types";
import { GET_YOUR_FAV_AUDIOS_KEY } from "~/lib/hooks/api/keys";

export function useYourFavoriteAudios(): UseInfinitePaginationReturnType<Audio> {
  async function fetcher(offset = 0): Promise<OffsetPagedList<Audio>> {
    const { data } = await request<OffsetPagedList<Audio>>({
      method: "get",
      url: "me/favorite/audios",
      params: { offset },
    });
    return data;
  }

  return useInfinitePagination(GET_YOUR_FAV_AUDIOS_KEY, fetcher);
}
