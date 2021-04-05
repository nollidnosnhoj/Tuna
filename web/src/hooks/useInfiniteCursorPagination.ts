import { useMemo } from "react";
import { useInfiniteQuery, UseInfiniteQueryOptions } from "react-query";
import { ErrorResponse, CursorPagedList } from "../lib/types";

export default function useInfiniteCursorPagination<TItem>(
  key: string, 
  fetcher: (cursor: number) => Promise<CursorPagedList<TItem>>, 
  params?: Record<string, any>,
  options?: UseInfiniteQueryOptions<CursorPagedList<TItem>, ErrorResponse>
) {
  const {
    data,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage
  } = useInfiniteQuery<CursorPagedList<TItem>, ErrorResponse>([key, params], ({ pageParam = 0 }) => 
    fetcher(pageParam), {
      getNextPageParam: (lastPage) => lastPage.next,
      getPreviousPageParam: (firstPage) => firstPage.previous,
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