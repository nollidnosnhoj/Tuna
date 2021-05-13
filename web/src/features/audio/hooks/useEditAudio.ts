import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { useAuth } from "~/lib/hooks/useAuth";
import api from "~/lib/api";
import { AudioDetail, AudioRequest } from "../types";

export function useEditAudio(id: string): UseMutationResult<AudioDetail> {
  const queryClient = useQueryClient();
  const { accessToken } = useAuth();
  const updateAudio = async (input: AudioRequest): Promise<AudioDetail> => {
    const { data } = await api.put<AudioDetail>(`audios/${id}`, input, {
      accessToken,
    });
    return data;
  };

  return useMutation(updateAudio, {
    onSuccess: (data) => {
      queryClient.setQueryData<AudioDetail>([`audios`, id], data);
      queryClient.invalidateQueries(`audios`);
    },
  });
}
