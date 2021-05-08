import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { useAuth } from "~/lib/hooks/useAuth";
import api from "~/lib/api";

type ImageResult = {
  image: string;
};

export function useAddAudioPicture(id: string): UseMutationResult<ImageResult> {
  const queryClient = useQueryClient();
  const { accessToken } = useAuth();
  const uploadArtwork = async (imageData: string): Promise<ImageResult> => {
    const { data } = await api.patch<ImageResult>(
      `audios/${id}/picture`,
      { imageData },
      { accessToken }
    );
    return data;
  };

  return useMutation(uploadArtwork, {
    onSuccess() {
      queryClient.invalidateQueries(`audios`);
      queryClient.invalidateQueries([`audios`, id], { exact: true });
    },
  });
}
