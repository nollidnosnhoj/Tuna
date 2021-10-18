import {
  useInfinitePagination,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import request from "~/lib/http";
import { AudioView, OffsetPagedList } from "~/lib/types";
import { GET_YOUR_AUDIOS_KEY } from "~/lib/hooks/api/keys";

export function useYourAudios(): UseInfinitePaginationReturnType<AudioView> {
  const fetcher = async (
    offset: number
  ): Promise<OffsetPagedList<AudioView>> => {
    const { data } = await request<OffsetPagedList<AudioView>>({
      method: "get",
      url: "me/audios",
      params: {
        offset,
      },
    });
    return data;
  };

  return useInfinitePagination(GET_YOUR_AUDIOS_KEY, fetcher);
}
