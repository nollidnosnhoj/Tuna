import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { GET_YOUR_AUDIOS_KEY } from "~/features/auth/hooks/useYourAudios";
import { useUser } from "~/features/user/hooks";
import { GET_USER_AUDIOS_QUERY_KEY } from "~/features/user/hooks/useGetUserAudios";
import { useAudioQueue } from "~/lib/stores";
import { removeAudioRequest } from "../api";
import { AudioId } from "../api/types";
import { GET_AUDIO_QUERY_KEY } from "./useGetAudio";
import { GET_AUDIO_LIST_QUERY_KEY } from "./useGetAudioList";

export function useRemoveAudio(id: AudioId): UseMutationResult<void> {
  const { clearQueue } = useAudioQueue();
  const queryClient = useQueryClient();
  const { user } = useUser();
  const removeAudio = async (): Promise<void> => {
    await removeAudioRequest(id);
  };

  return useMutation(removeAudio, {
    onSuccess() {
      clearQueue(id);
      queryClient.invalidateQueries(GET_AUDIO_LIST_QUERY_KEY);
      queryClient.invalidateQueries(GET_AUDIO_QUERY_KEY(id), { exact: true });
      if (user) {
        queryClient.invalidateQueries(GET_USER_AUDIOS_QUERY_KEY(user.username));
        queryClient.invalidateQueries(GET_YOUR_AUDIOS_KEY);
      }
    },
  });
}
