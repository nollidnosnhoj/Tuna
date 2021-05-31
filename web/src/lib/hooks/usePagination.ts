import { useMemo, useState } from "react";
import {
  QueryKey,
  useQuery,
  UseQueryOptions,
  UseQueryResult,
} from "react-query";
import { ErrorResponse, PagedList } from "../types";

type PaginationQueryFunction<TItem> = (
  page: number
) => Promise<PagedList<TItem>>;

export interface UsePaginationResultType<TItem>
  extends Omit<UseQueryResult<PagedList<TItem>>, "data"> {
  items: TItem[];
  page: number;
  setPage: (page: number) => void;
  totalCount: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export type UsePaginationOptions<TItem> = UseQueryOptions<PagedList<TItem>>;

export function usePagination<TItem>(
  key: QueryKey,
  func: PaginationQueryFunction<TItem>,
  initialPage = 1,
  options?: UseQueryOptions<PagedList<TItem>>
): UsePaginationResultType<TItem> {
  const [page, setPage] = useState(initialPage);
  const { data, ...result } = useQuery<PagedList<TItem>, ErrorResponse>(
    key,
    () => func(page),
    {
      keepPreviousData: true,
      ...options,
    }
  );

  const items = useMemo<TItem[]>(() => {
    return data ? data.items : [];
  }, [data]);

  const totalCount = useMemo<number>(() => {
    return data ? data.count : 0;
  }, [data]);

  const totalPages = useMemo<number>(() => {
    return data ? data.totalPages : 0;
  }, [data]);

  const hasPrevious = useMemo<boolean>(() => {
    return data ? data.hasPrevious : false;
  }, [data]);

  const hasNext = useMemo<boolean>(() => {
    return data ? data.hasNext : false;
  }, [data]);

  const changePage = (num: number): void => {
    setPage(num);
  };

  return {
    items,
    page,
    setPage: changePage,
    totalCount,
    totalPages,
    hasPrevious,
    hasNext,
    ...result,
  };
}
