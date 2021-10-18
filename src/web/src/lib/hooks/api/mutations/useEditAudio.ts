import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import request from "~/lib/http";
import { AudioRequest, AudioView, ID } from "~/lib/types";
import { useUser } from "~/components/providers/UserProvider";
import {
  GET_AUDIO_LIST_QUERY_KEY,
  GET_AUDIO_QUERY_KEY,
  GET_USER_AUDIOS_QUERY_KEY,
  GET_YOUR_AUDIOS_KEY,
} from "~/lib/hooks/api/keys";

export function useEditAudio(audioId: ID): UseMutationResult<AudioView> {
  const queryClient = useQueryClient();
  const { user } = useUser();
  const updateAudio = async (input: AudioRequest): Promise<AudioView> => {
    const { data } = await request<AudioView>({
      url: `audios/${audioId}`,
      method: "put",
      data: input,
    });
    return data;
  };

  return useMutation(updateAudio, {
    onSuccess: (data) => {
      queryClient.setQueryData<AudioView>(GET_AUDIO_QUERY_KEY(audioId), data);
      queryClient.invalidateQueries(GET_AUDIO_LIST_QUERY_KEY);
      if (user) {
        queryClient.invalidateQueries(GET_USER_AUDIOS_QUERY_KEY(user.userName));
        queryClient.invalidateQueries(GET_YOUR_AUDIOS_KEY);
      }
    },
  });
}
