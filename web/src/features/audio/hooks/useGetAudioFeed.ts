import useInfinitePagination, {
  UseInfinitePaginationOptions,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks/useInfinitePagination";
import { AudioData } from "../types";

type UseGetAudioFeedParams = {
  size?: number;
};

export function useGetAudioFeed(
  params: UseGetAudioFeedParams = {},
  options: UseInfinitePaginationOptions<AudioData> = {}
): UseInfinitePaginationReturnType<AudioData> {
  return useInfinitePagination<AudioData>("me/feed", params, options);
}
