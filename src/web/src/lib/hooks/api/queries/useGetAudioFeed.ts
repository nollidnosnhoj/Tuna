import { QueryKey } from "react-query";
import {
  useInfinitePagination,
  UseInfinitePaginationOptions,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import { useUser } from "~/components/providers/UserProvider";
import request from "~/lib/http";
import { AudioView, OffsetPagedList } from "~/lib/types";

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