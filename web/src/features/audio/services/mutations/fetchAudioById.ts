import api, { FetchAudioOptions } from '~/utils/api';
import { AudioDetail } from '~/features/audio/types';

export async function fetchAudioById(id: string, options: FetchAudioOptions = {}) {
  const { data } = await api.get<AudioDetail>(`audios/${id}`, undefined, {
    accessToken: options.accessToken
  });
  return data;
}