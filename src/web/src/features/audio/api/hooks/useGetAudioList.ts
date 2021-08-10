import {
  useInfiniteCursorPagination,
  UseInfiniteCursorPaginationOptions,
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks";
import { getAudiosRequest } from "..";
import { AudioView } from "../types";

export const GET_AUDIO_LIST_QUERY_KEY = "audios";

export function useGetAudioList(
  params: Record<string, string | boolean | number> = {},
  options: UseInfiniteCursorPaginationOptions<AudioView> = {}
): UseInfiniteCursorPaginationReturnType<AudioView> {
  return useInfiniteCursorPagination<AudioView>(
    GET_AUDIO_LIST_QUERY_KEY,
    (cursor) => getAudiosRequest(cursor, params),
    options
  );
}
