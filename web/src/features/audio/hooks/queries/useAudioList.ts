import { UseInfiniteQueryOptions } from 'react-query';
import { CursorPagedList } from "~/lib/types";
import { Audio } from '~/features/audio/types';
import { fetch, fetchPages } from '~/utils/api';
import useInfiniteCursorPagination from '~/hooks/useInfiniteCursorPagination';


export default function useGetAudioListInfinite(params: Record<string, any> = {}, size: number = 15, options?: UseInfiniteQueryOptions<CursorPagedList<Audio>>) {
  Object.assign(params, { size });
  return useInfiniteCursorPagination<Audio>('audios', (cursor) => {
    if (cursor === 0) cursor = undefined;
    return fetch('audios', {...params, cursor: cursor})
  }, params, options);
}
