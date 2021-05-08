import { useMutation, useQueryClient } from 'react-query';
import { useAuth } from '~/lib/contexts/AuthContext';
import api from '~/lib/api';
import { AudioDetail } from '../../types';


export function useEditAudio(id: string) {
  const queryClient = useQueryClient();
  const { accessToken } = useAuth();
  const updateAudio = async (input: object) => {
    const { data } = await api.put<AudioDetail>(`audios/${id}`, input, { accessToken });
    return data;
  };

  return useMutation(updateAudio, {
    onSuccess: (data) => {
      queryClient.setQueryData<AudioDetail>([`audios`, id], data);
      queryClient.invalidateQueries(`audios`);
    }
  });
}
