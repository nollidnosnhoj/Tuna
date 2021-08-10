import {
  useInfiniteCursorPagination,
  UseInfiniteCursorPaginationOptions,
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks";
import { getAudiosRequest } from "..";
import { AudioView } from "../types";

export const GET_TAG_AUDIO_LIST_QUERY_KEY = "audios";

export function useGetTagAudioList(
  tag: string,
  params: Record<string, any> = {},
  options: UseInfiniteCursorPaginationOptions<AudioView> = {}
): UseInfiniteCursorPaginationReturnType<AudioView> {
  return useInfiniteCursorPagination<AudioView>(
    GET_TAG_AUDIO_LIST_QUERY_KEY,
    (cursor) => getAudiosRequest(cursor, { ...params, tag: tag }),
    options
  );
}
