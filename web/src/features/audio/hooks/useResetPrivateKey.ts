import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { resetPrivateKey } from "../api";
import { GET_AUDIO_QUERY_KEY } from "./useGetAudio";

export function useResetPrivateKey(
  audioId: number
): UseMutationResult<string, unknown, void> {
  const qc = useQueryClient();
  return useMutation<string>(() => resetPrivateKey(audioId), {
    onSuccess() {
      qc.invalidateQueries(GET_AUDIO_QUERY_KEY(audioId));
    },
  });
}
