import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { ErrorResponse } from "~/lib/types";
import { resetAudioSecret } from "..";
import { AudioId } from "../types";
import { GET_AUDIO_QUERY_KEY } from "./useGetAudio";

export function useResetAudioSecret(
  audioId: AudioId
): UseMutationResult<{ secret: string }, ErrorResponse, undefined> {
  const qc = useQueryClient();
  return useMutation(() => resetAudioSecret(audioId), {
    onSuccess() {
      qc.invalidateQueries(GET_AUDIO_QUERY_KEY(audioId));
    },
  });
}
