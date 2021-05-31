import { useMemo } from "react";
import {
  QueryKey,
  useInfiniteQuery,
  UseInfiniteQueryOptions,
  UseInfiniteQueryResult,
} from "react-query";
import { fetch, FetchRequestOptions } from "../api";
import { CursorPagedList } from "../types";

type InfiniteCursorPaginationQueryFunction<TItem> = (
  cursor: string
) => Promise<CursorPagedList<TItem>>;

export interface UseInfiniteCursorPaginationReturnType<TItem>
  extends Omit<UseInfiniteQueryResult<CursorPagedList<TItem>>, "data"> {
  items: TItem[];
}

export type UseInfiniteCursorPaginationOptions<TItem> = UseInfiniteQueryOptions<
  CursorPagedList<TItem>
>;

export const fetchCursorList = <TItem>(
  route: string,
  cursor?: string,
  queryParams?: Record<string, unknown>,
  fetchOptions?: FetchRequestOptions
): Promise<CursorPagedList<TItem>> => {
  return fetch<CursorPagedList<TItem>>(
    route,
    { ...queryParams, cursor: cursor },
    fetchOptions
  );
};

export function useInfiniteCursorPagination<TItem>(
  key: QueryKey,
  func: InfiniteCursorPaginationQueryFunction<TItem>,
  options?: UseInfiniteCursorPaginationOptions<TItem>
): UseInfiniteCursorPaginationReturnType<TItem> {
  const { data, ...result } = useInfiniteQuery<CursorPagedList<TItem>>(
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
  }, [data]);

  return {
    items,
    ...result,
  };
}
