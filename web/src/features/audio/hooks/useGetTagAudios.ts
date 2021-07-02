import {
  useInfiniteCursorPagination,
  UseInfiniteCursorPaginationOptions,
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks";
import { fetchAudiosHandler } from "../api";
import { AudioData } from "../types";

export const GET_TAG_AUDIO_LIST_QUERY_KEY = "audios";

export function useGetTagAudioList(
  tag: string,
  params: Record<string, any> = {},
  options: UseInfiniteCursorPaginationOptions<AudioData> = {}
): UseInfiniteCursorPaginationReturnType<AudioData> {
  return useInfiniteCursorPagination<AudioData>(
    GET_TAG_AUDIO_LIST_QUERY_KEY,
    (cursor) => fetchAudiosHandler(cursor, { ...params, tag: tag }),
    options
  );
}
