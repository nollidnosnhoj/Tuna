import { useMutation } from 'react-query';
import { useAuth } from '~/lib/contexts/AuthContext';
import api from '~/lib/api';
import { AudioDetail, CreateAudioRequest } from '../../types';


export function usePublishAudio() {
  const { accessToken } = useAuth();
  const uploadAudio = async (request: CreateAudioRequest) => {
    const { data } = await api.post<AudioDetail>('audios', request, { accessToken });
    return data;
  };

  return useMutation(uploadAudio);
}
