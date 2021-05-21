import { useMutation, UseMutationResult } from "react-query";
import { useAuth } from "~/features/auth/hooks/useAuth";
import api from "~/lib/api";
import { AudioDetailData, CreateAudioRequest } from "../types";

export function useCreateAudio(): UseMutationResult<
  AudioDetailData,
  unknown,
  CreateAudioRequest,
  unknown
> {
  const { accessToken } = useAuth();
  const createAudio = async (
    request: CreateAudioRequest
  ): Promise<AudioDetailData> => {
    const { data } = await api.post<AudioDetailData>("audios", request, {
      accessToken,
    });
    return data;
  };

  return useMutation(createAudio);
}
