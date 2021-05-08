import { UseInfiniteQueryOptions } from 'react-query';
import useInfinitePagination from "~/lib/hooks/useInfinitePagination";
import { PagedList } from "~/lib/types";
import { Audio } from '~/features/audio/types';
import { fetchPages } from '~/lib/api';
import { useAuth } from '~/lib/contexts/AuthContext';

interface UseGetAudioInfiniteOptions extends UseInfiniteQueryOptions<PagedList<Audio>> {
  params?: Record<string, any>
}

export function useGetAudioListInfinite(key: string, options: UseGetAudioInfiniteOptions = {}) {
  const { accessToken } = useAuth();
  const { params, ...queryOptions } = options;
  return useInfinitePagination<Audio>(key, (page) => fetchPages(key, params, page, { accessToken }), params, queryOptions);
}
