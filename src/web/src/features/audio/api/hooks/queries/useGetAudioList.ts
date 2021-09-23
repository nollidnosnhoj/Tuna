import { useCallback } from "react";
import {
  useInfiniteCursorPagination,
  UseInfiniteCursorPaginationOptions,
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks";
import request from "~/lib/http";
import { CursorPagedList } from "~/lib/types";
import { AudioView } from "../../types";

export const GET_AUDIO_LIST_QUERY_KEY = "audios";

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
