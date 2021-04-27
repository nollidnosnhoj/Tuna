import { useMutation } from 'react-query';
import { useAuth } from '~/contexts/AuthContext';
import api from '~/utils/api';
import { AudioDetail, CreateAudioRequest } from '../../types';


export function useCreateAudio() {
  const { accessToken } = useAuth();
  const uploadAudio = async (request: CreateAudioRequest) => {
    const { data } = await api.post<AudioDetail>('audios', request, { accessToken });
    return data;
  };

  return useMutation(uploadAudio);
}
