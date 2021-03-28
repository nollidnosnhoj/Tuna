import { UseQueryOptions } from 'react-query';
import usePagination from "~/hooks/usePagination";
import { PagedList } from "~/lib/types";
import { Audio } from '~/features/audio/types';
import { fetchPages } from '~/utils/api';


export function useGetAudioListPagination(key: string, params: Record<string, any> = {}, size: number = 15, options?: UseQueryOptions<PagedList<Audio>>) {
  Object.assign(params, { size });
  return usePagination(key, (page) => fetchPages(key, params, page), params, options);
}
