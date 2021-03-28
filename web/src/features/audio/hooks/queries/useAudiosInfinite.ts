import { UseInfiniteQueryOptions } from 'react-query';
import useInfinitePagination from "~/hooks/useInfinitePagination";
import { PagedList } from "~/lib/types";
import { Audio } from '~/features/audio/types';
import { fetchPages } from '~/utils/api';


export function useGetAudioListInfinite(key: string, params: Record<string, any> = {}, size: number = 15, options?: UseInfiniteQueryOptions<PagedList<Audio>>) {
  Object.assign(params, { size });
  return useInfinitePagination<Audio>(key, (page) => fetchPages(key, params, page), params, options);
}
