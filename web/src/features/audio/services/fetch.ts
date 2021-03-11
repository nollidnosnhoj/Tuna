import api, { FetchAudioOptions } from '~/utils/api';
import { AudioDetail } from '~/features/audio/types';

export const fetchAudioById = async (id: string, options: FetchAudioOptions = {}) => {
  const { data } = await api.get<AudioDetail>(`audios/${id}`, { 
    accessToken: options.accessToken
  });
  return data;
}