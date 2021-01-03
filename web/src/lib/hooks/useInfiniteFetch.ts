import { useMemo } from 'react';
import { useSWRInfinite } from 'swr'
import { stringify as queryStringify } from 'query-string'

export interface useInfiniteFetchOptions {
  size?: number,
  params?: Record<string, any>
}

const useInfiniteFetch = <TItem = any, TError = any>(url: string, options: useInfiniteFetchOptions = {}) => {

  const { size = 15, params = {}} = options;

  const getKey = (pageIndex: number, previousPageData: TItem[]) => {
    if (previousPageData && !previousPageData.length) return null;

    let key = `${url}?page=${pageIndex + 1}&size=${size}`

    const queryString = queryStringify(params);

    return (queryString) ? (key + `&${queryString}`) : key
  }

  const {
    data,
    size: page,
    setSize: setPage,
    error,
    mutate,
    isValidating,
  } = useSWRInfinite<TItem[], TError>(getKey);

  const isLoadingInitialData = useMemo<boolean>(() => {
    return !data && !error;
  }, [data, error]);

  const isLoadingMore = useMemo<boolean>(() => {
    return (
      isLoadingInitialData ||
      (page > 1 && data && typeof data[page - 1] === "undefined")
    );
  }, [page, data]);

  const isEmpty = data?.[0]?.length === 0;

  const isReachingEnd = useMemo<boolean>(() => {
    return isEmpty || (data && data[data.length - 1]?.length < size);
  }, [data]);

  const isRefreshing = useMemo<boolean>(() => {
    return isValidating && data && data.length === page;
  }, [data]);

  return {
    data,
    page,
    setPage,
    error,
    mutate,
    isValidating,
    isLoadingInitialData,
    isLoadingMore,
    isEmpty,
    isReachingEnd,
    isRefreshing
  }
}

export default useInfiniteFetch;