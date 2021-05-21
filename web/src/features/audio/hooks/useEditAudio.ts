import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { useAuth } from "~/features/auth/hooks/useAuth";
import api from "~/lib/api";
import { AudioDetailData, AudioRequest } from "../types";

export function useEditAudio(id: string): UseMutationResult<AudioDetailData> {
  const queryClient = useQueryClient();
  const { accessToken } = useAuth();
  const updateAudio = async (input: AudioRequest): Promise<AudioDetailData> => {
    const { data } = await api.put<AudioDetailData>(`audios/${id}`, input, {
      accessToken,
    });
    return data;
  };

  return useMutation(updateAudio, {
    onSuccess: (data) => {
      queryClient.setQueryData<AudioDetailData>([`audios`, id], data);
      queryClient.invalidateQueries(`audios`);
    },
  });
}
