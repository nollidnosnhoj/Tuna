import { useMutation, UseMutationResult } from "react-query";
import request from "~/lib/http";
import { AudioView, CreateAudioRequest } from "../types";

export function useCreateAudio(): UseMutationResult<
  AudioView,
  unknown,
  CreateAudioRequest,
  unknown
> {
  return useMutation(async (input) => {
    const { data } = await request<AudioView>({
      url: "audios",
      method: "post",
      data: input,
    });
    return data;
  });
}
