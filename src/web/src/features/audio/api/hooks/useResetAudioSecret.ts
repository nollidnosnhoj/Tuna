import { useCallback } from "react";
import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import request from "~/lib/http";
import { ErrorResponse } from "~/lib/types";
import { AudioId } from "../types";
import { GET_AUDIO_QUERY_KEY } from "./useGetAudio";

export function useResetAudioSecret(
  audioId: AudioId
): UseMutationResult<{ secret: string }, ErrorResponse, undefined> {
  const qc = useQueryClient();
  const mutate = useCallback(async () => {
    const { data } = await request<{ secret: string }>({
      method: "PATCH",
      url: `audios/${audioId}/reset`,
    });
    return data;
  }, [audioId]);
  return useMutation(mutate, {
    onSuccess() {
      qc.invalidateQueries(GET_AUDIO_QUERY_KEY(audioId));
    },
  });
}
