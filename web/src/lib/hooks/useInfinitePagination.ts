import { useMemo } from "react";
import {
  useInfiniteQuery,
  UseInfiniteQueryOptions,
  UseInfiniteQueryResult,
} from "react-query";
import { fetchPages } from "../api";
import { PagedList } from "../types";
import { useAuth } from "../../features/auth/hooks/useAuth";

export interface UseInfinitePaginationReturnType<TItem>
  extends Omit<UseInfiniteQueryResult<PagedList<TItem>>, "data"> {
  items: TItem[];
}

export type UseInfinitePaginationOptions<TItem> = UseInfiniteQueryOptions<
  PagedList<TItem>
>;

export function useInfinitePagination<TItem>(
  key: string,
  params?: Record<string, unknown>,
  options?: UseInfiniteQueryOptions<PagedList<TItem>>
): UseInfinitePaginationReturnType<TItem> {
  const { accessToken } = useAuth();
  const { data, ...result } = useInfiniteQuery<PagedList<TItem>>(
    [key, params],
    ({ pageParam = 1 }) =>
      fetchPages<TItem>(key, params, pageParam, { accessToken }),
    {
      getNextPageParam: (lastPage) =>
        lastPage.hasNext ? lastPage.page + 1 : undefined,
      getPreviousPageParam: (firstPage) =>
        firstPage.hasPrevious ? firstPage.page - 1 : undefined,
      ...options,
    }
  );

  const items = useMemo<TItem[]>(() => {
    return data
      ? ([] as TItem[]).concat(...data.pages.map((x) => x.items))
      : [];
  }, [data?.pages.length, data?.pageParams.length]);

  return {
    items,
    ...result,
  };
}
