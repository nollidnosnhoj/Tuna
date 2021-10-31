import {
  useInfinitePagination,
  UseInfinitePaginationOptions,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import { useUser } from "~/components/providers/UserProvider";
import request from "~/lib/http";
import { Audio, OffsetPagedList } from "~/lib/types";
import { GET_AUDIO_FEED_QUERY_KEY } from "~/lib/hooks/api/keys";

export function useGetAudioFeed(
  options: UseInfinitePaginationOptions<Audio> = {}
): UseInfinitePaginationReturnType<Audio> {
  const { isLoggedIn } = useUser();
  const fetcher = async (offset: number): Promise<OffsetPagedList<Audio>> => {
    const { data } = await request<OffsetPagedList<Audio>>({
      method: "get",
      url: "me/audios/feed",
      params: { offset },
    });
    return data;
  };
  return useInfinitePagination<Audio>(GET_AUDIO_FEED_QUERY_KEY, fetcher, {
    enabled: isLoggedIn,
    ...options,
  });
}
