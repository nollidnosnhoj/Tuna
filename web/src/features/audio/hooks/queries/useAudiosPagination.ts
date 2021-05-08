import { UseQueryOptions } from "react-query";
import usePagination, {
  UsePaginationResultType,
} from "~/lib/hooks/usePagination";
import { PagedList } from "~/lib/types";
import { Audio } from "~/features/audio/types";
import { fetchPages } from "~/lib/api";
import { useAuth } from "~/lib/hooks/useAuth";

interface UseGetAudioPaginationListOptions
  extends UseQueryOptions<PagedList<Audio>> {
  params?: Record<string, unknown>;
  page?: number;
}

export function useGetAudioListPagination(
  key: string,
  options: UseGetAudioPaginationListOptions = { page: 1 }
): UsePaginationResultType<Audio> {
  const { accessToken } = useAuth();
  const { params, page: initPage, ...queryOptions } = options;
  return usePagination<Audio>(
    key,
    (page) => fetchPages(key, params, page, { accessToken }),
    params,
    initPage,
    queryOptions
  );
}
