import { useMutation, UseMutationResult } from "react-query";
import { createAudioRequest } from "../api";
import { AudioView, CreateAudioRequest } from "../api/types";

export function useCreateAudio(): UseMutationResult<
  AudioView,
  unknown,
  CreateAudioRequest,
  unknown
> {
  return useMutation(createAudioRequest);
}
