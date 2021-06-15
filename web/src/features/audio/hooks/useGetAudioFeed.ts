import { QueryKey } from "react-query";
import {
  useInfinitePagination,
  UseInfinitePaginationOptions,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import { AudioData } from "../types";
import { fetchAudioFeedHandler } from "../api";

export const GET_AUDIO_FEED_QUERY_KEY: QueryKey = "feed";

export function useGetAudioFeed(
  options: UseInfinitePaginationOptions<AudioData> = {}
): UseInfinitePaginationReturnType<AudioData> {
  return useInfinitePagination<AudioData>(
    GET_AUDIO_FEED_QUERY_KEY,
    (page) => fetchAudioFeedHandler(page),
    options
  );
}
