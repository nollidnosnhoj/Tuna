import api, { FetchAudioOptions } from '~/utils/api';
import { Audio } from '~/features/audio/types';

export const fetchAudioById = async (id: string, options: FetchAudioOptions = {}) => {
  const { data } = await api.get<Audio>(`audios/${id}`, { 
    accessToken: options.accessToken
  });
  return data;
}