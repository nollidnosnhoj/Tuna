import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { useAuth } from "~/features/auth/hooks";
import { useUser } from "~/features/user/hooks";
import { GET_USER_AUDIOS_QUERY_KEY } from "~/features/user/hooks/useGetUserAudios";
import api from "~/lib/api";
import { AudioDetailData, AudioRequest } from "../types";
import { GET_AUDIO_QUERY_KEY } from "./useGetAudio";
import { GET_AUDIO_LIST_QUERY_KEY } from "./useGetAudioList";

export function useEditAudio(id: string): UseMutationResult<AudioDetailData> {
  const queryClient = useQueryClient();
  const { accessToken } = useAuth();
  const { user } = useUser();
  const updateAudio = async (input: AudioRequest): Promise<AudioDetailData> => {
    const { data } = await api.put<AudioDetailData>(`audios/${id}`, input, {
      accessToken,
    });
    return data;
  };

  return useMutation(updateAudio, {
    onSuccess: (data) => {
      queryClient.setQueryData<AudioDetailData>(GET_AUDIO_QUERY_KEY(id), data);
      queryClient.invalidateQueries(GET_AUDIO_LIST_QUERY_KEY);
      if (user) {
        queryClient.invalidateQueries(GET_USER_AUDIOS_QUERY_KEY(user.id));
      }
    },
  });
}
