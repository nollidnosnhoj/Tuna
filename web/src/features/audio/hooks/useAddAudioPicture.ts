import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { useAuth } from "~/lib/hooks/useAuth";
import api from "~/lib/api";
import { ErrorResponse } from "~/lib/types";
import { AudioDetailData } from "../types";

export function useAddAudioPicture(
  id: string
): UseMutationResult<AudioDetailData, ErrorResponse, string> {
  const queryClient = useQueryClient();
  const { accessToken } = useAuth();
  const uploadArtwork = async (imageData: string): Promise<AudioDetailData> => {
    const { data } = await api.patch<AudioDetailData>(
      `audios/${id}/picture`,
      { data: imageData },
      { accessToken }
    );
    return data;
  };

  return useMutation(uploadArtwork, {
    onSuccess(data) {
      queryClient.setQueryData<AudioDetailData>([`audios`, id], data);
      queryClient.invalidateQueries(`audios`);
    },
  });
}
