import { UseInfiniteQueryOptions } from 'react-query';
import { CursorPagedList } from "~/lib/types";
import { Audio } from '~/features/audio/types';
import { fetch, fetchPages } from '~/lib/api';
import useInfiniteCursorPagination from '~/lib/hooks/useInfiniteCursorPagination';
import { useAuth } from '~/lib/contexts/AuthContext';


export default function useGetAudioListInfinite(params: Record<string, any> = {}, size: number = 15, options?: UseInfiniteQueryOptions<CursorPagedList<Audio>>) {
  const { accessToken } = useAuth();
  Object.assign(params, { size });
  return useInfiniteCursorPagination<Audio>('audios', (cursor) => {
    if (!cursor) cursor = undefined;
    return fetch('audios', {...params, cursor: cursor }, { accessToken })
  }, params, options);
}
