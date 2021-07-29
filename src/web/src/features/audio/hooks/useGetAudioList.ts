import {
  useInfiniteCursorPagination,
  UseInfiniteCursorPaginationOptions,
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks";
import { getAudiosRequest } from "../api";
import { AudioData } from "../types";

export const GET_AUDIO_LIST_QUERY_KEY = "audios";

export function useGetAudioList(
  params: Record<string, string | boolean | number> = {},
  options: UseInfiniteCursorPaginationOptions<AudioData> = {}
): UseInfiniteCursorPaginationReturnType<AudioData> {
  return useInfiniteCursorPagination<AudioData>(
    GET_AUDIO_LIST_QUERY_KEY,
    (cursor) => getAudiosRequest(cursor, params),
    options
  );
}
