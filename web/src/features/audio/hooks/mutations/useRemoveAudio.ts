import { useMutation, useQueryClient } from 'react-query';
import { useAuth } from '~/contexts/AuthContext';
import api from '~/utils/api';


export function useRemoveAudio(id: number) {
  const queryClient = useQueryClient();
  const { accessToken } = useAuth();
  const removeAudio = async () => await api.delete(`audios/${id}`, { accessToken });

  return useMutation(removeAudio, {
    onSuccess() {
      queryClient.invalidateQueries(`audios`);
      queryClient.invalidateQueries([`audios`, id], { exact: true });
    }
  });
}
