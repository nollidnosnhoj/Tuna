import { useMutation, useQueryClient } from 'react-query';
import api from '~/utils/api';
import { AudioDetail } from '../../types';


export function useEditAudio(id: number) {
  const queryClient = useQueryClient();
  const updateAudio = async (input: object) => {
    const { data } = await api.put<AudioDetail>(`audios/${id}`, input);
    return data;
  };

  return useMutation(updateAudio, {
    onSuccess: (data) => {
      queryClient.setQueryData<AudioDetail>([`audios`, id], data);
      queryClient.invalidateQueries(`audios`);
    }
  });
}
