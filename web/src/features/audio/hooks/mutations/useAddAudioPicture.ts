import { useMutation, useQueryClient } from 'react-query';
import { addAudioPicture } from '../../services/mutations/addAudioPicture';


export function useAddAudioPicture(id: number) {
  const queryClient = useQueryClient();
  const uploadArtwork = async (data: string) => {
    return await addAudioPicture(id, data);
  };

  return useMutation(uploadArtwork, {
    onSuccess(data) {
      queryClient.invalidateQueries(`audios`);
      queryClient.invalidateQueries([`audios`, id], { exact: true });
    }
  });
}
