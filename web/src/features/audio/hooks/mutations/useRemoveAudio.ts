import { useMutation, useQueryClient } from 'react-query';
import { useAuth } from '~/lib/contexts/AuthContext';
import api from '~/lib/api';


export function useRemoveAudio(id: string) {
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
