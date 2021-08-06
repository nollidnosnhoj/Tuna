import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { GET_YOUR_AUDIOS_KEY } from "~/features/auth/hooks/useYourAudios";
import { useUser } from "~/features/user/hooks";
import { GET_USER_AUDIOS_QUERY_KEY } from "~/features/user/hooks/useGetUserAudios";
import { updateAudioDetailsRequest } from "../api";
import { AudioData, AudioId, AudioRequest } from "../types";
import { GET_AUDIO_QUERY_KEY } from "./useGetAudio";
import { GET_AUDIO_LIST_QUERY_KEY } from "./useGetAudioList";

export function useEditAudio(id: AudioId): UseMutationResult<AudioData> {
  const queryClient = useQueryClient();
  const { user } = useUser();
  const updateAudio = async (input: AudioRequest): Promise<AudioData> => {
    return updateAudioDetailsRequest(id, input);
  };

  return useMutation(updateAudio, {
    onSuccess: (data) => {
      queryClient.setQueryData<AudioData>(GET_AUDIO_QUERY_KEY(id), data);
      queryClient.invalidateQueries(GET_AUDIO_LIST_QUERY_KEY);
      if (user) {
        queryClient.invalidateQueries(GET_USER_AUDIOS_QUERY_KEY(user.id));
        queryClient.invalidateQueries(GET_YOUR_AUDIOS_KEY);
      }
    },
  });
}
