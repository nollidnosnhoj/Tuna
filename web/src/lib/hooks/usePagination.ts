import { useMemo, useState } from "react";
import { useQuery } from "react-query";
import { ErrorResponse, PagedList } from "../types";

export default function usePagination<TItem>(key: string, fetcher: (page: number) => Promise<PagedList<TItem>>, params?: Record<string, any>) {
  const [page, setPage] = useState(1);
  const {
    data,
    error,
    isLoading,
    isError,
    isFetching,
    isPreviousData
  } = useQuery<PagedList<TItem>, ErrorResponse>([key, { ...params, page }], () => fetcher(page), {
    keepPreviousData: true
  });

  const items = useMemo<TItem[]>(() => {
    return data ? data.items : []
  }, [data]);

  const totalCount = useMemo<number>(() => {
    return data ? data.count : 0;
  }, [data]);

  const totalPages = useMemo<number>(() => {
    return data ? data.totalPages : 0;
  }, [data]);

  const hasPrevious = useMemo<boolean>(() => {
    return data ? data.hasPrevious : false
  }, [data]);

  const hasNext = useMemo<boolean>(() => {
    return data ? data.hasNext : false
  }, [data]);

  return { 
    items,
    error,
    isLoading,
    isError,
    isFetching,
    isPreviousData,
    page,
    setPage,
    totalCount,
    totalPages,
    hasPrevious,
    hasNext
  }
}