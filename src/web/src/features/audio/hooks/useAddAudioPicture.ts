import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { AudioView, AudioId } from "../api/types";
import { GET_AUDIO_QUERY_KEY } from "./useGetAudio";
import { GET_AUDIO_LIST_QUERY_KEY } from "./useGetAudioList";
import { uploadAudioPictureRequest } from "../api";
import { useUser } from "~/features/user/hooks";
import { GET_USER_AUDIOS_QUERY_KEY } from "~/features/user/hooks/useGetUserAudios";
import { ErrorResponse, ImageUploadResponse } from "~/lib/types";

export function useAddAudioPicture(
  id: AudioId
): UseMutationResult<ImageUploadResponse, ErrorResponse, string> {
  const queryClient = useQueryClient();
  const { user } = useUser();
  const uploadArtwork = async (
    imageData: string
  ): Promise<ImageUploadResponse> => {
    return await uploadAudioPictureRequest(id, imageData);
  };

  return useMutation(uploadArtwork, {
    onSuccess(data) {
      const audio = queryClient.getQueryData<AudioView>(
        GET_AUDIO_QUERY_KEY(id)
      );
      if (audio) {
        queryClient.setQueryData<AudioView>(GET_AUDIO_QUERY_KEY(id), {
          ...audio,
          picture: data.url,
        });
      }
      queryClient.invalidateQueries(GET_AUDIO_LIST_QUERY_KEY);
      if (user) {
        queryClient.invalidateQueries(GET_USER_AUDIOS_QUERY_KEY(user.username));
      }
    },
  });
}
