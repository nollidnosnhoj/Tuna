import { useMutation, UseMutationResult } from "react-query";
import { createAudioHandler } from "../api";
import { AudioDetailData, CreateAudioRequest } from "../types";

export function useCreateAudio(): UseMutationResult<
  AudioDetailData,
  unknown,
  CreateAudioRequest,
  unknown
> {
  return useMutation(createAudioHandler);
}
