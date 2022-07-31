import { useMemo } from "react";
import {
  QueryKey,
  useInfiniteQuery,
  UseInfiniteQueryOptions,
  UseInfiniteQueryResult,
} from "react-query";
import { OffsetPagedList } from "../../types";

type InfinitePaginationQueryFunction<TItem> = (
  offset: number
) => Promise<OffsetPagedList<TItem>>;

export interface UseInfinitePaginationReturnType<TItem>
  extends Omit<UseInfiniteQueryResult<OffsetPagedList<TItem>>, "data"> {
  items: TItem[];
}

export type UseInfinitePaginationOptions<TItem> = UseInfiniteQueryOptions<
  OffsetPagedList<TItem>
>;

export function useInfinitePagination<TItem>(
  key: QueryKey,
  func: InfinitePaginationQueryFunction<TItem>,
  options?: UseInfiniteQueryOptions<OffsetPagedList<TItem>>
): UseInfinitePaginationReturnType<TItem> {
  const { data, ...result } = useInfiniteQuery<OffsetPagedList<TItem>>(
    key,
    ({ pageParam = undefined }) => func(pageParam),
    {
      getNextPageParam: (lastPage) => lastPage.next,
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
