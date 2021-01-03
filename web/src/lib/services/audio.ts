import useSWR, { mutate } from 'swr'
import useInfiniteFetch, { useInfiniteFetchOptions } from '../hooks/useInfiniteFetch'
import { Audio, AudioListItem, AudioRequest, AudioSearchType, ErrorResponse } from '../types';
import request from '../request';



export const useAudio = (id: string, initialData?: Audio) => {
  const { data, isValidating: isLoading, error, mutate } = useSWR<Audio, ErrorResponse>(`audios/${id}`, { 
    revalidateOnFocus: false,
    initialData 
  });

  return { data, isLoading, error, mutate };
}

interface useAudiosInfiniteOptions extends useInfiniteFetchOptions {
  type: AudioSearchType
  username?: string
}

export const useAudiosInfinite = (options: useAudiosInfiniteOptions = { type: 'audios' }) => {
  const { type, size, params = {}, username } = options;

  let url = 'audios'

  if (type === 'favorites' && username) {
    url = `users/${username}/favorites`
  }

  if (type === 'user' && username) {
    url = `users/${username}/audios`
  }

  return useInfiniteFetch<AudioListItem, ErrorResponse>(url, { size, params });
}

export const uploadAudio = async (formData: FormData) => {
  const { data } = await request<Audio>('audios', {
    method: 'post',
    body: formData
  });

  return data;
}

export const deleteAudio = async (id: string | number) => {
  mutate(`audios/${id}`, null, false);
  await request(`audios/${id}`, { method: 'post' });
  mutate(`audios/${id}`);
}

export const updateAudio = async (audio: Audio, inputs: AudioRequest) => {
  const { data } = await request<Audio>(`audios/${audio.id}`, {
    method: 'patch',
    body: inputs
  });
  mutate(`audios/${audio.id}`, data, false);
}