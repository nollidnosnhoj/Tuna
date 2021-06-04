import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { useAuth } from "~/features/auth/hooks";
import { useUser } from "~/features/user/hooks";
import api from "~/lib/api";
import { ErrorResponse } from "~/lib/types";
import { AudioDetailData } from "../types";
import { GET_USER_AUDIOS_QUERY_KEY } from "~/features/user/hooks/useGetUserAudios";
import { GET_AUDIO_QUERY_KEY } from "./useGetAudio";
import { GET_AUDIO_LIST_QUERY_KEY } from "./useGetAudioList";

export function useAddAudioPicture(
  id: string
): UseMutationResult<AudioDetailData, ErrorResponse, string> {
  const queryClient = useQueryClient();
  const { accessToken } = useAuth();
  const [user] = useUser();
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
      queryClient.setQueryData<AudioDetailData>(GET_AUDIO_QUERY_KEY(id), data);
      queryClient.invalidateQueries(GET_AUDIO_LIST_QUERY_KEY);
      if (user) {
        queryClient.invalidateQueries(GET_USER_AUDIOS_QUERY_KEY(user.id));
      }
    },
  });
}
