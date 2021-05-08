import { useMemo } from "react";
import {
  useInfiniteQuery,
  UseInfiniteQueryOptions,
  UseInfiniteQueryResult,
} from "react-query";
import { PagedList } from "../types";

export interface UseInfinitePaginationReturnType<TItem>
  extends Omit<UseInfiniteQueryResult<PagedList<TItem>>, "data"> {
  items: TItem[];
}

export default function useInfinitePagination<TItem>(
  key: string,
  fetcher: (page: number) => Promise<PagedList<TItem>>,
  params?: Record<string, unknown>,
  options?: UseInfiniteQueryOptions<PagedList<TItem>>
): UseInfinitePaginationReturnType<TItem> {
  const { data, ...result } = useInfiniteQuery<PagedList<TItem>>(
    [key, params],
    ({ pageParam = 1 }) => fetcher(pageParam),
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
