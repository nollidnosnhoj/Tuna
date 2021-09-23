import { AudioView } from "~/features/audio/api/types";
import {
  useInfinitePagination,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import request from "~/lib/http";
import { OffsetPagedList } from "~/lib/types";

export const GET_YOUR_AUDIOS_KEY = "your_audios";

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
