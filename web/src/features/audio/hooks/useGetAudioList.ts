import useInfiniteCursorPagination, {
  UseInfiniteCursorPaginationOptions,
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks/useInfiniteCursorPagination";
import { AudioData } from "../types";

export type UseGetAudioListParams = {
  tag?: string;
  size?: number;
};

export function useGetAudioList(
  params: UseGetAudioListParams = {},
  options: UseInfiniteCursorPaginationOptions<AudioData> = {}
): UseInfiniteCursorPaginationReturnType<AudioData> {
  return useInfiniteCursorPagination<AudioData>("audios", params, options);
}
