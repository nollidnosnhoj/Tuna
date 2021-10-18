import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { GET_AUDIO_QUERY_KEY } from "../queries/useGetAudio";
import { GET_AUDIO_LIST_QUERY_KEY } from "../queries/useGetAudioList";
import { useUser } from "~/components/providers/UserProvider";
import { GET_USER_AUDIOS_QUERY_KEY } from "~/lib/hooks/api/queries/useGetUserAudios";
import { AudioView, ErrorResponse, ID, ImageUploadResponse } from "~/lib/types";
import request from "~/lib/http";

export function useRemoveAudioPicture(
  id: ID
): UseMutationResult<void, ErrorResponse, void> {
  const queryClient = useQueryClient();
  const { user } = useUser();
  const uploadArtwork = async (): Promise<void> => {
    await request<ImageUploadResponse>({
      url: `audios/${id}/picture`,
      method: "DELETE",
    });
  };

  return useMutation(uploadArtwork, {
    onSuccess() {
      const audio = queryClient.getQueryData<AudioView>(
        GET_AUDIO_QUERY_KEY(id)
      );
      if (audio) {
        queryClient.setQueryData<AudioView>(GET_AUDIO_QUERY_KEY(id), {
          ...audio,
          picture: "",
        });
      }
      queryClient.invalidateQueries(GET_AUDIO_LIST_QUERY_KEY);
      if (user) {
        queryClient.invalidateQueries(GET_USER_AUDIOS_QUERY_KEY(user.userName));
      }
    },
  });
}