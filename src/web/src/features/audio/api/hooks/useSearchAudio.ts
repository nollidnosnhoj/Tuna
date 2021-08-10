import { QueryKey } from "react-query";
import {
  usePagination,
  UsePaginationOptions,
  UsePaginationResultType,
} from "~/lib/hooks";
import { AudioView } from "../types";
import { SearchAudioParams, searchAudiosRequest } from "..";

export const SEARCH_AUDIO_QUERY_KEY = (
  term: string,
  tags?: string
): QueryKey => ["searchAudios", { q: term, tags: tags }];

export function useSearchAudio(
  searchTerm: string,
  page: number,
  params: SearchAudioParams = {},
  options: UsePaginationOptions<AudioView> = {}
): UsePaginationResultType<AudioView> {
  const { tags } = params;
  return usePagination<AudioView>(
    SEARCH_AUDIO_QUERY_KEY(searchTerm, tags),
    (page) => searchAudiosRequest(searchTerm, page, params),
    page,
    options
  );
}
