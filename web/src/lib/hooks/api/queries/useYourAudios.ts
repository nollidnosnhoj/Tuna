import {
  useInfinitePagination,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import request from "~/lib/http";
import { Audio, OffsetPagedList } from "~/lib/types";
import { GET_YOUR_AUDIOS_KEY } from "~/lib/hooks/api/keys";

export function useYourAudios(): UseInfinitePaginationReturnType<Audio> {
  const fetcher = async (offset: number): Promise<OffsetPagedList<Audio>> => {
    const { data } = await request<OffsetPagedList<Audio>>({
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
