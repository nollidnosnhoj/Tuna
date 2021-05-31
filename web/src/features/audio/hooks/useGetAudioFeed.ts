import { QueryKey } from "react-query";
import { useAuth } from "~/features/auth/hooks";
import { fetchPages } from "~/lib/api";
import {
  useInfinitePagination,
  UseInfinitePaginationOptions,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import { AudioData } from "../types";

export const GET_AUDIO_FEED_QUERY_KEY: QueryKey = "feed";

export function useGetAudioFeed(
  options: UseInfinitePaginationOptions<AudioData> = {}
): UseInfinitePaginationReturnType<AudioData> {
  const { accessToken } = useAuth();
  return useInfinitePagination<AudioData>(
    GET_AUDIO_FEED_QUERY_KEY,
    (page) => fetchPages<AudioData>("me/feed", {}, page, { accessToken }),
    options
  );
}
