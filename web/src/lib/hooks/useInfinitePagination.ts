import { useMemo } from "react";
import { useInfiniteQuery } from "react-query";
import { PagedList, ErrorResponse } from "../types";

export default function useInfinitePagination<TItem>(key: string, fetcher: (page: number) => Promise<PagedList<TItem>>, params?: Record<string, any>) {
  const {
    data,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useInfiniteQuery<PagedList<TItem>, ErrorResponse>([key, params], ({ pageParam = 1 }) => 
    fetcher(pageParam), {
      getNextPageParam: (lastPage) => {
        return lastPage.hasNext ? lastPage.page + 1 : undefined
      },
      getPreviousPageParam: (firstPage) => {
        return firstPage.hasPrevious ? firstPage.page - 1 : undefined
      }
    });

  const items = useMemo<TItem[]>(() => {
    return data ? [].concat(...data.pages.map((x) => x.items)) : [];
  }, [data]);

  return {
    items,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage
  }
}