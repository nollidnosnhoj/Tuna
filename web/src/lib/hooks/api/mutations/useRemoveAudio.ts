import { useCallback } from "react";
import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { useUser } from "~/components/providers/UserProvider";
import { useAudioPlayer } from "~/lib/stores";
import request from "~/lib/http";
import { ID } from "~/lib/types";
import {
  GET_AUDIO_LIST_QUERY_KEY,
  GET_AUDIO_QUERY_KEY,
  GET_USER_AUDIOS_QUERY_KEY,
  GET_YOUR_AUDIOS_KEY,
} from "~/lib/hooks/api/keys";

export function useRemoveAudio(audioId: ID): UseMutationResult<void> {
  const { removeAudioIdFromQueue: removeAudioFromQueue } = useAudioPlayer();
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
      await queryClient.invalidateQueries(GET_AUDIO_LIST_QUERY_KEY);
      await queryClient.invalidateQueries(GET_AUDIO_QUERY_KEY(audioId), {
        exact: true,
      });
      if (user) {
        await queryClient.invalidateQueries(
          GET_USER_AUDIOS_QUERY_KEY(user.userName)
        );
        await queryClient.invalidateQueries(GET_YOUR_AUDIOS_KEY);
      }
    },
  });
}
