import { useMutation, UseMutationResult } from "react-query";
import { createAudioRequest } from "../api";
import { AudioDetailData, CreateAudioRequest } from "../types";

export function useCreateAudio(): UseMutationResult<
  AudioDetailData,
  unknown,
  CreateAudioRequest,
  unknown
> {
  return useMutation(createAudioRequest);
}
