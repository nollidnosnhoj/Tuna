import { useCallback } from "react";
import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { GET_YOUR_AUDIOS_KEY } from "~/features/auth/api/hooks/queries/useYourAudios";
import { useUser } from "~/features/user/hooks";
import { GET_USER_AUDIOS_QUERY_KEY } from "~/features/user/api/hooks/queries/useGetUserAudios";
import { useAudioQueue } from "~/lib/stores";
import { GET_AUDIO_QUERY_KEY } from "../queries/useGetAudio";
import { GET_AUDIO_LIST_QUERY_KEY } from "../queries/useGetAudioList";
import request from "~/lib/http";
import { ID } from "~/lib/types";

export function useRemoveAudio(audioId: ID): UseMutationResult<void> {
  const { removeAudioIdFromQueue: removeAudioFromQueue } = useAudioQueue();
  const queryClient = useQueryClient();
  const { user } = useUser();
  const removeAudio = useCallback(async () => {
    await request({
      method: "delete",
      url: `audios/${audioId}`,
    });
  }, [audioId]);

  return useMutation(removeAudio, {
    async onSuccess() {
      await removeAudioFromQueue(audioId);
      queryClient.invalidateQueries(GET_AUDIO_LIST_QUERY_KEY);
      queryClient.invalidateQueries(GET_AUDIO_QUERY_KEY(audioId), {
        exact: true,
      });
      if (user) {
        queryClient.invalidateQueries(GET_USER_AUDIOS_QUERY_KEY(user.userName));
        queryClient.invalidateQueries(GET_YOUR_AUDIOS_KEY);
      }
    },
  });
}
