import { useMemo } from "react";
import {
  useInfiniteQuery,
  UseInfiniteQueryOptions,
  UseInfiniteQueryResult,
} from "react-query";
import { CursorPagedList } from "../types";

export interface UseInfiniteCursorPaginationReturnType<TItem>
  extends Omit<UseInfiniteQueryResult<CursorPagedList<TItem>>, "data"> {
  items: TItem[];
}

export default function useInfiniteCursorPagination<TItem>(
  key: string,
  fetcher: (cursor?: string) => Promise<CursorPagedList<TItem>>,
  params?: Record<string, unknown>,
  options?: UseInfiniteQueryOptions<CursorPagedList<TItem>>
): UseInfiniteCursorPaginationReturnType<TItem> {
  const { data, ...result } = useInfiniteQuery<CursorPagedList<TItem>>(
    [key, params],
    ({ pageParam = undefined }) => fetcher(pageParam),
    {
      getNextPageParam: (lastPage) => lastPage.next,
      ...options,
    }
  );

  const items = useMemo<TItem[]>(() => {
    return data
      ? ([] as TItem[]).concat(...data.pages.map((x) => x.items))
      : [];
  }, [data]);

  return {
    items,
    ...result,
  };
}
