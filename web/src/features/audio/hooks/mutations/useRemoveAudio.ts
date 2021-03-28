import { useMutation, useQueryClient } from 'react-query';
import api from '~/utils/api';


export function useRemoveAudio(id: number) {
  const queryClient = useQueryClient();
  const removeAudio = async () => await api.delete(`audios/${id}`);

  return useMutation(removeAudio, {
    onSuccess() {
      queryClient.invalidateQueries(`audios`);
      queryClient.invalidateQueries([`audios`, id], { exact: true });
    }
  });
}
