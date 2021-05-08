import { useQuery, UseQueryOptions } from 'react-query';
import { ErrorResponse } from "~/lib/types";
import { AudioDetail } from '~/features/audio/types';
import { fetchAudioById } from '../../services/mutations/fetchAudioById';
import { useAuth } from "~/lib/hooks/useAuth";

export function useGetAudio(id: string, options: UseQueryOptions<AudioDetail, ErrorResponse> = {}) {
  const { accessToken } = useAuth();
  return useQuery<AudioDetail, ErrorResponse>(['audios', id], () => fetchAudioById(id, { accessToken }), options);
}
