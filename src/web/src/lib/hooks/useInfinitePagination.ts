import { useMemo } from "react";
import {
  QueryKey,
  useInfiniteQuery,
  UseInfiniteQueryOptions,
  UseInfiniteQueryResult,
} from "react-query";
import { PagedList } from "../types";

type InfinitePaginationQueryFunction<TItem> = (
  page: number
) => Promise<PagedList<TItem>>;

export interface UseInfinitePaginationReturnType<TItem>
  extends Omit<UseInfiniteQueryResult<PagedList<TItem>>, "data"> {
  items: TItem[];
}

export type UseInfinitePaginationOptions<TItem> = UseInfiniteQueryOptions<
  PagedList<TItem>
>;

export function useInfinitePagination<TItem>(
  key: QueryKey,
  func: InfinitePaginationQueryFunction<TItem>,
  options?: UseInfiniteQueryOptions<PagedList<TItem>>
): UseInfinitePaginationReturnType<TItem> {
  const { data, ...result } = useInfiniteQuery<PagedList<TItem>>(
    key,
    ({ pageParam = 1 }) => func(pageParam),
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
