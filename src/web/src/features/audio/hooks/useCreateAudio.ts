import { useMutation, UseMutationResult } from "react-query";
import { createAudioRequest } from "../api";
import { AudioData, CreateAudioRequest } from "../types";

export function useCreateAudio(): UseMutationResult<
  AudioData,
  unknown,
  CreateAudioRequest,
  unknown
> {
  return useMutation(createAudioRequest);
}
