import { useCallback } from "react";
import {
  useInfiniteCursorPagination,
  UseInfiniteCursorPaginationOptions,
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks";
import request from "~/lib/http";
import { AudioView, CursorPagedList } from "~/lib/types";
import { GET_AUDIO_LIST_QUERY_KEY } from "~/lib/hooks/api/keys";

export function useGetAudioList(
  params: Record<string, string | boolean | number> = {},
  options: UseInfiniteCursorPaginationOptions<AudioView> = {}
): UseInfiniteCursorPaginationReturnType<AudioView> {
  const fetcher = useCallback(
    async (cursor: number) => {
      const { data } = await request<CursorPagedList<AudioView>>({
        method: "get",
        url: "audios",
        params: { ...params, cursor: cursor },
      });
      return data;
    },
    [params]
  );

  return useInfiniteCursorPagination<AudioView>(
    GET_AUDIO_LIST_QUERY_KEY,
    fetcher,
    options
  );
}
