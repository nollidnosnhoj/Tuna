import { useMutation, UseMutationResult } from "react-query";
import { useAuth } from "~/lib/hooks/useAuth";
import api from "~/lib/api";
import { AudioDetail, CreateAudioRequest } from "../../types";

export function usePublishAudio(): UseMutationResult<AudioDetail> {
  const { accessToken } = useAuth();
  const uploadAudio = async (
    request: CreateAudioRequest
  ): Promise<AudioDetail> => {
    const { data } = await api.post<AudioDetail>("audios", request, {
      accessToken,
    });
    return data;
  };

  return useMutation(uploadAudio);
}
