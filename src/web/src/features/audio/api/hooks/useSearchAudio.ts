import { QueryKey } from "react-query";
import {
  usePagination,
  UsePaginationOptions,
  UsePaginationResultType,
} from "~/lib/hooks";
import { AudioView } from "../types";
import { useCallback } from "react";
import request from "~/lib/http";
import { PagedList } from "~/lib/types";

export const SEARCH_AUDIO_QUERY_KEY = (
  term: string,
  tags?: string
): QueryKey => ["searchAudios", { q: term, tags: tags }];

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
