import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { GET_YOUR_AUDIOS_KEY } from "~/features/auth/api/hooks/useYourAudios";
import { useUser } from "~/features/user/hooks";
import { GET_USER_AUDIOS_QUERY_KEY } from "~/features/user/api/hooks/useGetUserAudios";
import { AudioView, AudioId, AudioRequest } from "../types";
import { GET_AUDIO_QUERY_KEY } from "./useGetAudio";
import { GET_AUDIO_LIST_QUERY_KEY } from "./useGetAudioList";
import request from "~/lib/http";

export function useEditAudio(id: AudioId): UseMutationResult<AudioView> {
  const queryClient = useQueryClient();
  const { user } = useUser();
  const updateAudio = async (input: AudioRequest): Promise<AudioView> => {
    const { data } = await request<AudioView>({
      url: `audios/${id}`,
      method: "put",
      data: input,
    });
    return data;
  };

  return useMutation(updateAudio, {
    onSuccess: (data) => {
      queryClient.setQueryData<AudioView>(GET_AUDIO_QUERY_KEY(id), data);
      queryClient.invalidateQueries(GET_AUDIO_LIST_QUERY_KEY);
      if (user) {
        queryClient.invalidateQueries(GET_USER_AUDIOS_QUERY_KEY(user.userName));
        queryClient.invalidateQueries(GET_YOUR_AUDIOS_KEY);
      }
    },
  });
}
