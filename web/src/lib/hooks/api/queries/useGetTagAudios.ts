import { useCallback } from "react";
import {
  useInfiniteCursorPagination,
  UseInfiniteCursorPaginationOptions,
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks";
import request from "~/lib/http";
import { Audio, CursorPagedList } from "~/lib/types";
import { GET_TAG_AUDIO_LIST_QUERY_KEY } from "~/lib/hooks/api/keys";

export function useGetTagAudioList(
  tag: string,
  params: Record<string, any> = {},
  options: UseInfiniteCursorPaginationOptions<Audio> = {}
): UseInfiniteCursorPaginationReturnType<Audio> {
  const fetcher = useCallback(
    async (cursor: number) => {
      const { data } = await request<CursorPagedList<Audio>>({
        method: "get",
        url: "audios",
        params: { ...params, cursor: cursor, tag },
      });
      return data;
    },
    [params, tag]
  );

  return useInfiniteCursorPagination<Audio>(
    GET_TAG_AUDIO_LIST_QUERY_KEY,
    fetcher,
    options
  );
}
