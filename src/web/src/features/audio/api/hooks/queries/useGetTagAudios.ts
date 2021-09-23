import { useCallback } from "react";
import {
  useInfiniteCursorPagination,
  UseInfiniteCursorPaginationOptions,
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks";
import request from "~/lib/http";
import { CursorPagedList } from "~/lib/types";
import { AudioView } from "../../types";

export const GET_TAG_AUDIO_LIST_QUERY_KEY = "audios";

export function useGetTagAudioList(
  tag: string,
  params: Record<string, any> = {},
  options: UseInfiniteCursorPaginationOptions<AudioView> = {}
): UseInfiniteCursorPaginationReturnType<AudioView> {
  const fetcher = useCallback(
    async (cursor: number) => {
      const { data } = await request<CursorPagedList<AudioView>>({
        method: "get",
        url: "audios",
        params: { ...params, cursor: cursor, tag },
      });
      return data;
    },
    [params, tag]
  );

  return useInfiniteCursorPagination<AudioView>(
    GET_TAG_AUDIO_LIST_QUERY_KEY,
    fetcher,
    options
  );
}
