import {
  usePagination,
  UsePaginationOptions,
  UsePaginationResultType,
} from "~/lib/hooks";
import { useCallback } from "react";
import request from "~/lib/http";
import { AudioView, PagedList } from "~/lib/types";
import { SEARCH_AUDIO_QUERY_KEY } from "~/lib/hooks/api/keys";

export function useSearchAudio(
  searchTerm: string,
  page: number,
  params: Record<string, any> = {},
  options: UsePaginationOptions<AudioView> = {}
): UsePaginationResultType<AudioView> {
  const { tags } = params;
  const fetcher = useCallback(
    async (pageNumber: number) => {
      const { data } = await request<PagedList<AudioView>>({
        method: "get",
        url: "search/audios",
        params: {
          ...params,
          q: searchTerm,
          page: pageNumber,
        },
      });
      return data;
    },
    [searchTerm, params]
  );
  return usePagination<AudioView>(
    SEARCH_AUDIO_QUERY_KEY(searchTerm, tags),
    fetcher,
    page,
    options
  );
}
