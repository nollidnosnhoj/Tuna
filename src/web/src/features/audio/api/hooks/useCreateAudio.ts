import { useMutation, UseMutationResult } from "react-query";
import { createAudioRequest } from "..";
import { AudioView, CreateAudioRequest } from "../types";

export function useCreateAudio(): UseMutationResult<
  AudioView,
  unknown,
  CreateAudioRequest,
  unknown
> {
  return useMutation(createAudioRequest);
}
