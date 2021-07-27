import { QueryKey } from "react-query";
import {
  useInfinitePagination,
  UseInfinitePaginationOptions,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import { AudioData } from "../types";
import { getAudioFeedRequest } from "../api";
import { useUser } from "~/features/user/hooks";

export const GET_AUDIO_FEED_QUERY_KEY: QueryKey = "feed";

export function useGetAudioFeed(
  options: UseInfinitePaginationOptions<AudioData> = {}
): UseInfinitePaginationReturnType<AudioData> {
  const { isLoggedIn } = useUser();
  return useInfinitePagination<AudioData>(
    GET_AUDIO_FEED_QUERY_KEY,
    (page) => getAudioFeedRequest(page),
    {
      enabled: isLoggedIn,
      ...options,
    }
  );
}
