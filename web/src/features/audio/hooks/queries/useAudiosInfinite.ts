import { UseInfiniteQueryOptions } from "react-query";
import useInfinitePagination, {
  UseInfinitePaginationReturnType,
} from "~/lib/hooks/useInfinitePagination";
import { PagedList } from "~/lib/types";
import { Audio } from "~/features/audio/types";
import { fetchPages } from "~/lib/api";
import { useAuth } from "~/lib/hooks/useAuth";

interface UseGetAudioInfiniteOptions
  extends UseInfiniteQueryOptions<PagedList<Audio>> {
  params?: Record<string, unknown>;
}

export function useGetAudioListInfinite(
  key: string,
  options: UseGetAudioInfiniteOptions = {}
): UseInfinitePaginationReturnType<Audio> {
  const { accessToken } = useAuth();
  const { params, ...queryOptions } = options;
  return useInfinitePagination<Audio>(
    key,
    (page) => fetchPages(key, params, page, { accessToken }),
    params,
    queryOptions
  );
}
