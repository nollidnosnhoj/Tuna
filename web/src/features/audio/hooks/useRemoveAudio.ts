import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { useAuth } from "~/features/auth/hooks/useAuth";
import api from "~/lib/api";
import { useAudioPlayer } from "~/lib/stores";

export function useRemoveAudio(id: string): UseMutationResult<void> {
  const { removeFromQueueByAudioId } = useAudioPlayer();
  const queryClient = useQueryClient();
  const { accessToken } = useAuth();
  const removeAudio = async (): Promise<void> => {
    await api.delete(`audios/${id}`, { accessToken });
  };

  return useMutation(removeAudio, {
    onSuccess() {
      removeFromQueueByAudioId(id);
      queryClient.invalidateQueries(`audios`);
      queryClient.invalidateQueries([`audios`, id], { exact: true });
    },
  });
}
