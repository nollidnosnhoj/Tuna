import { useQuery, UseQueryOptions } from 'react-query';
import { ErrorResponse } from "~/lib/types";
import { AudioDetail } from '~/features/audio/types';
import { fetchAudioById } from '../../services/mutations/fetchAudioById';

export function useGetAudio(id: string, options: UseQueryOptions<AudioDetail, ErrorResponse> = {}) {
  return useQuery<AudioDetail, ErrorResponse>(['audios', id], () => fetchAudioById(id), options);
}
