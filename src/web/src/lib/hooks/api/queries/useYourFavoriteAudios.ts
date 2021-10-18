import {
  useInfinitePagination,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import request from "~/lib/http";
import { AudioView, OffsetPagedList } from "~/lib/types";
import { GET_YOUR_FAV_AUDIOS_KEY } from "~/lib/hooks/api/keys";

export function useYourFavoriteAudios(): UseInfinitePaginationReturnType<AudioView> {
  async function fetcher(offset = 0): Promise<OffsetPagedList<AudioView>> {
    const { data } = await request<OffsetPagedList<AudioView>>({
      method: "get",
      url: "me/favorite/audios",
      params: { offset },
    });
    return data;
  }

  return useInfinitePagination(GET_YOUR_FAV_AUDIOS_KEY, fetcher);
}
