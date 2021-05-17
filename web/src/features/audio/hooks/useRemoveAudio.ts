import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { useAuth } from "~/lib/hooks/useAuth";
import api from "~/lib/api";
import { useAudioPlayer } from "~/lib/hooks/useAudioPlayer";

export function useRemoveAudio(id: string): UseMutationResult<void> {
  const { dispatch } = useAudioPlayer();
  const queryClient = useQueryClient();
  const { accessToken } = useAuth();
  const removeAudio = async (): Promise<void> => {
    await api.delete(`audios/${id}`, { accessToken });
  };

  return useMutation(removeAudio, {
    onSuccess() {
      dispatch({ type: "REMOVE_AUDIO_ID_FROM_QUEUE", payload: id });
      queryClient.invalidateQueries(`audios`);
      queryClient.invalidateQueries([`audios`, id], { exact: true });
    },
  });
}
