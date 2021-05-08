import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { useAuth } from "~/lib/hooks/useAuth";
import api from "~/lib/api";

export function useRemoveAudio(id: string): UseMutationResult<void> {
  const queryClient = useQueryClient();
  const { accessToken } = useAuth();
  const removeAudio = async (): Promise<void> => {
    await api.delete(`audios/${id}`, { accessToken });
  };

  return useMutation(removeAudio, {
    onSuccess() {
      queryClient.invalidateQueries(`audios`);
      queryClient.invalidateQueries([`audios`, id], { exact: true });
    },
  });
}
