import { QueryKey } from "react-query";
import {
  usePagination,
  UsePaginationOptions,
  UsePaginationResultType,
} from "~/lib/hooks";
import { AudioData } from "../types";
import { SearchAudioParams, searchAudiosHandler } from "../api";

export const SEARCH_AUDIO_QUERY_KEY = (
  term: string,
  tags?: string
): QueryKey => ["searchAudios", { q: term, tags: tags }];

export function useSearchAudio(
  searchTerm: string,
  page: number,
  params: SearchAudioParams = {},
  options: UsePaginationOptions<AudioData> = {}
): UsePaginationResultType<AudioData> {
  const { tags } = params;
  return usePagination<AudioData>(
    SEARCH_AUDIO_QUERY_KEY(searchTerm, tags),
    (page) => searchAudiosHandler(searchTerm, page, params),
    page,
    options
  );
}
