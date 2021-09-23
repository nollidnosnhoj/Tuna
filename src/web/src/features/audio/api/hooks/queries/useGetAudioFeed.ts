import { QueryKey } from "react-query";
import {
  useInfinitePagination,
  UseInfinitePaginationOptions,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import { AudioView } from "../../types";
import { useUser } from "~/features/user/hooks";
import request from "~/lib/http";
import { OffsetPagedList } from "~/lib/types";

export const GET_AUDIO_FEED_QUERY_KEY: QueryKey = "feed";

export function useGetAudioFeed(
  options: UseInfinitePaginationOptions<AudioView> = {}
): UseInfinitePaginationReturnType<AudioView> {
  const { isLoggedIn } = useUser();
  const fetcher = async (
    offset: number
  ): Promise<OffsetPagedList<AudioView>> => {
    const { data } = await request<OffsetPagedList<AudioView>>({
      method: "get",
      url: "me/audios/feed",
      params: { offset },
    });
    return data;
  };
  return useInfinitePagination<AudioView>(GET_AUDIO_FEED_QUERY_KEY, fetcher, {
    enabled: isLoggedIn,
    ...options,
  });
}
