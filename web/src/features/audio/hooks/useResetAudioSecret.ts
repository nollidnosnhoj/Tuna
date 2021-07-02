import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { resetAudioSecret } from "../api";
import { GET_AUDIO_QUERY_KEY } from "./useGetAudio";

export function useResetAudioSecret(
  audioId: number
): UseMutationResult<string, unknown, void> {
  const qc = useQueryClient();
  return useMutation<string>(() => resetAudioSecret(audioId), {
    onSuccess() {
      qc.invalidateQueries(GET_AUDIO_QUERY_KEY(audioId));
    },
  });
}
