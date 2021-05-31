import { useAuth } from "~/features/auth/hooks";
import {
  fetchCursorList,
  useInfiniteCursorPagination,
  UseInfiniteCursorPaginationOptions,
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks";
import { AudioData } from "../types";

export const GET_TAG_AUDIO_LIST_QUERY_KEY = "audios";

export function useGetAudioList(
  tag: string,
  params: Record<string, string | boolean | number> = {},
  options: UseInfiniteCursorPaginationOptions<AudioData> = {}
): UseInfiniteCursorPaginationReturnType<AudioData> {
  const { accessToken } = useAuth();
  return useInfiniteCursorPagination<AudioData>(
    GET_TAG_AUDIO_LIST_QUERY_KEY,
    (cursor) =>
      fetchCursorList<AudioData>(
        "audios",
        cursor,
        { ...params, tag: tag },
        { accessToken }
      ),
    options
  );
}
