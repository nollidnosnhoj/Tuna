import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { useAuth } from "~/lib/hooks/useAuth";
import api from "~/lib/api";
import { ErrorResponse } from "~/lib/types";

type ImageResult = {
  image: string;
};

export function useAddAudioPicture(
  id: string
): UseMutationResult<ImageResult, ErrorResponse, string> {
  const queryClient = useQueryClient();
  const { accessToken } = useAuth();
  const uploadArtwork = async (imageData: string): Promise<ImageResult> => {
    const { data } = await api.patch<ImageResult>(
      `audios/${id}/picture`,
      { data: imageData },
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
