import { useMemo } from "react";
import { useInfiniteQuery, UseInfiniteQueryOptions } from "react-query";
import { PagedList, ErrorResponse } from "../lib/types";

export default function useInfinitePagination<TItem>(
  key: string, 
  fetcher: (page: number) => Promise<PagedList<TItem>>, 
  params?: Record<string, any>,
  options?: UseInfiniteQueryOptions<PagedList<TItem>, ErrorResponse>
) {
  const {
    data,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage
  } = useInfiniteQuery<PagedList<TItem>, ErrorResponse>([key, params], ({ pageParam = 1 }) => 
    fetcher(pageParam), {
      getNextPageParam: (lastPage) => lastPage.hasNext 
        ? lastPage.page + 1 
        : undefined,
      getPreviousPageParam: (firstPage) => firstPage.hasPrevious 
        ? firstPage.page - 1 
        : undefined,
      ...options
    });

  const items = useMemo<TItem[]>(() => {
    return data ? ([] as TItem[]).concat(...(data.pages.map(x => x.items))) : [];
  }, [data]);

  return {
    items,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage
  }
}