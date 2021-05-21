import { useMemo } from "react";
import {
  useInfiniteQuery,
  UseInfiniteQueryOptions,
  UseInfiniteQueryResult,
} from "react-query";
import { fetch, FetchRequestOptions } from "../api";
import { CursorPagedList } from "../types";
import { useAuth } from "../../features/auth/hooks/useAuth";

export interface UseInfiniteCursorPaginationReturnType<TItem>
  extends Omit<UseInfiniteQueryResult<CursorPagedList<TItem>>, "data"> {
  items: TItem[];
}

export type UseInfiniteCursorPaginationOptions<TItem> = UseInfiniteQueryOptions<
  CursorPagedList<TItem>
>;

export const fetchCursorList = <TItem>(
  key: string,
  cursor?: string,
  queryParams?: Record<string, unknown>,
  fetchOptions?: FetchRequestOptions
): Promise<CursorPagedList<TItem>> => {
  return fetch<CursorPagedList<TItem>>(
    key,
    { ...queryParams, cursor: cursor },
    fetchOptions
  );
};

export default function useInfiniteCursorPagination<TItem>(
  key: string,
  params?: Record<string, unknown>,
  options?: UseInfiniteCursorPaginationOptions<TItem>
): UseInfiniteCursorPaginationReturnType<TItem> {
  const { accessToken } = useAuth();
  const { data, ...result } = useInfiniteQuery<CursorPagedList<TItem>>(
    [key, params],
    ({ pageParam = undefined }) =>
      fetchCursorList<TItem>(key, pageParam, params, { accessToken }),
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
