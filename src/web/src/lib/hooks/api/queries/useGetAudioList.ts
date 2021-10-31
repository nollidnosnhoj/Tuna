import { useCallback } from "react";
import {
  useInfiniteCursorPagination,
  UseInfiniteCursorPaginationOptions,
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks";
import request from "~/lib/http";
import { Audio, CursorPagedList } from "~/lib/types";
import { GET_AUDIO_LIST_QUERY_KEY } from "~/lib/hooks/api/keys";

export function useGetAudioList(
  params: Record<string, string | boolean | number> = {},
  options: UseInfiniteCursorPaginationOptions<Audio> = {}
): UseInfiniteCursorPaginationReturnType<Audio> {
  const fetcher = useCallback(
    async (cursor: number) => {
      const { data } = await request<CursorPagedList<Audio>>({
        method: "get",
        url: "audios",
        params: { ...params, cursor: cursor },
      });
      return data;
    },
    [params]
  );

  return useInfiniteCursorPagination<Audio>(
    GET_AUDIO_LIST_QUERY_KEY,
    fetcher,
    options
  );
}
