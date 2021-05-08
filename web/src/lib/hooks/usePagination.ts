import { useMemo, useState } from "react";
import { useQuery, UseQueryOptions, UseQueryResult } from "react-query";
import { ErrorResponse, PagedList } from "../types";

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

export default function usePagination<TItem>(
  key: string,
  fetcher: (page: number) => Promise<PagedList<TItem>>,
  params: Record<string, unknown> = {},
  initialPage = 1,
  options?: UseQueryOptions<PagedList<TItem>>
): UsePaginationResultType<TItem> {
  const [page, setPage] = useState(initialPage);
  const { data, ...result } = useQuery<PagedList<TItem>, ErrorResponse>(
    [key, { ...params, page }],
    () => fetcher(page),
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
