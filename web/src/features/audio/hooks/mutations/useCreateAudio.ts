import { useMutation } from 'react-query';
import api from '~/utils/api';
import { AudioDetail, CreateAudioRequest } from '../../types';


export function useCreateAudio() {
  const uploadAudio = async (request: CreateAudioRequest) => {
    const { data } = await api.post<AudioDetail>('audios', request);
    return data;
  };

  return useMutation(uploadAudio);
}
