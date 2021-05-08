import { useMemo } from "react";
import { useInfiniteQuery, UseInfiniteQueryOptions } from "react-query";
import { ErrorResponse, CursorPagedList } from "../types";

export default function useInfiniteCursorPagination<TItem>(
  key: string, 
  fetcher: (cursor?: string) => Promise<CursorPagedList<TItem>>, 
  params?: Record<string, any>,
  options?: UseInfiniteQueryOptions<CursorPagedList<TItem>, ErrorResponse>
) {
  const {
    data,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage
  } = useInfiniteQuery<CursorPagedList<TItem>, ErrorResponse>([key, params], ({ pageParam = undefined }) => fetcher(pageParam), {
      getNextPageParam: (lastPage) => lastPage.next,
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