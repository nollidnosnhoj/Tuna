import { useRouter } from "next/router";
import { Audio } from "~/features/audio/types";
import { fetchPages } from "~/lib/api";
import { useAuth } from "~/lib/hooks/useAuth";
import { useGetPageParam } from "~/lib/hooks/useGetPageParam";
import usePagination, {
  UsePaginationResultType,
} from "~/lib/hooks/usePagination";

interface UseAudioSearchQueryResult extends UsePaginationResultType<Audio> {
  searchQuery?: string | string[];
}

export function useAudioSearchQuery(): UseAudioSearchQueryResult {
  const { accessToken } = useAuth();
  const { query } = useRouter();
  const [page, queryParams] = useGetPageParam(query);
  const { q } = queryParams;

  const pageResults = usePagination<Audio>(
    "search/audios",
    (page: number) =>
      fetchPages<Audio>("search/audios", queryParams, page, {
        accessToken,
      }),
    queryParams,
    page
  );

  return {
    searchQuery: q,
    ...pageResults,
  };
}
