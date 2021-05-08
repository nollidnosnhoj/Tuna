import { UseQueryOptions } from 'react-query';
import usePagination from "~/lib/hooks/usePagination";
import { PagedList } from "~/lib/types";
import { Audio } from '~/features/audio/types';
import { fetchPages } from '~/lib/api';
import { useAuth } from '~/lib/contexts/AuthContext';

interface UseGetAudioPaginationListOptions extends UseQueryOptions<PagedList<Audio>> {
  params?: Record<string, any>
  page?: number;
}

export function useGetAudioListPagination(key: string, options: UseGetAudioPaginationListOptions = { page: 1 }) {
  const { accessToken } = useAuth();
  const { params, page: initPage, ...queryOptions } = options;
  return usePagination(key, (page) => fetchPages(key, params, page, { accessToken }), params, initPage, queryOptions);
}
