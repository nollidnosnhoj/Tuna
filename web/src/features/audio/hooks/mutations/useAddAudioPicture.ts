import { useMutation, useQueryClient } from 'react-query';
import { useAuth } from '~/contexts/AuthContext';
import api from '~/utils/api'


export function useAddAudioPicture(id: number) {
  const queryClient = useQueryClient();
  const { accessToken } = useAuth();
  const uploadArtwork = async (data: string) => {
    return await api.patch<{ image: string; }>(`audios/${id}/picture`, { data }, { accessToken });
  };

  return useMutation(uploadArtwork, {
    onSuccess(data) {
      queryClient.invalidateQueries(`audios`);
      queryClient.invalidateQueries([`audios`, id], { exact: true });
    }
  });
}
