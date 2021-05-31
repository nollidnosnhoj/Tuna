import { QueryKey } from "react-query";
import { useAuth } from "~/features/auth/hooks";
import { fetchPages } from "~/lib/api";
import {
  usePagination,
  UsePaginationOptions,
  UsePaginationResultType,
} from "~/lib/hooks";
import { AudioData } from "../types";

type UseSearchAudioParams = {
  tags?: string;
  size?: number;
};

export const SEARCH_AUDIO_QUERY_KEY = (
  term: string,
  tags?: string
): QueryKey => ["searchAudios", { q: term, tags: tags }];

export function useSearchAudio(
  searchTerm: string,
  page: number,
  params: UseSearchAudioParams = {},
  options: UsePaginationOptions<AudioData> = {}
): UsePaginationResultType<AudioData> {
  const { tags, ...otherParams } = params;
  const { accessToken } = useAuth();
  return usePagination<AudioData>(
    SEARCH_AUDIO_QUERY_KEY(searchTerm, tags),
    (page) =>
      fetchPages<AudioData>(
        "search/audios",
        {
          q: searchTerm,
          tags,
          ...otherParams,
        },
        page,
        { accessToken }
      ),
    page,
    options
  );
}
