import { useRouter } from "next/router";
import { UseQueryOptions } from "react-query";
import { Audio } from "~/features/audio/types";
import { fetchPages } from "~/lib/api";
import { useAuth } from "~/lib/hooks/useAuth";
import { useGetPageParam } from "~/lib/hooks/useGetPageParam";
import usePagination, {
  UsePaginationResultType,
} from "~/lib/hooks/usePagination";
import { PagedList } from "~/lib/types";

interface UseAudioSearchQueryResult extends UsePaginationResultType<Audio> {
  searchQuery?: string | string[];
}

export function useAudioSearchQuery(
  size?: number,
  queryOptions?: UseQueryOptions<PagedList<Audio>>
): UseAudioSearchQueryResult {
  const { accessToken } = useAuth();
  const { query } = useRouter();
  const [page, queryParams] = useGetPageParam(query);
  const { q } = queryParams;
  if (typeof size !== "undefined" && size > 0) {
    Object.assign(queryParams, { size });
  }

  const pageResults = usePagination<Audio>(
    "search/audios",
    (page: number) =>
      fetchPages<Audio>("search/audios", queryParams, page, {
        accessToken,
      }),
    queryParams,
    page,
    queryOptions
  );

  return {
    searchQuery: q,
    ...pageResults,
  };
}
