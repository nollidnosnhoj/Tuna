import { UseInfiniteQueryOptions, useQuery, UseQueryOptions } from 'react-query';
import useInfinitePagination from "~/hooks/useInfinitePagination";
import usePagination from "~/hooks/usePagination";
import { ErrorResponse, PagedList } from "~/lib/types";
import { Audio } from '~/features/audio/types';
import { fetchAudioById } from '../services/fetch';
import { fetchPages } from '~/utils/api';

export const useAudio = (id: string, options: UseQueryOptions<Audio, ErrorResponse> = {}) => {
  return useQuery<Audio, ErrorResponse>(['audios', id], () => fetchAudioById(id), options);
}

export const useAudiosInfinite = (key: string, params: Record<string, any> = {}, size: number = 15, options?: UseInfiniteQueryOptions<PagedList<Audio>>) => {
  Object.assign(params, { size });
  return useInfinitePagination<Audio>(key, (page) => fetchPages(key, params, page), params, options);
}

export const useAudiosPagination = (key: string, params: Record<string, any> = {}, size: number = 15, options?: UseQueryOptions<PagedList<Audio>>) => {
  Object.assign(params, { size });
  return usePagination(key, (page) => fetchPages(key, params, page), params, options);
}