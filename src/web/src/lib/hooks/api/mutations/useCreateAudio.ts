import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import request from "~/lib/http";
import { AudioView, CreateAudioRequest } from "~/lib/types";
import { useUser } from "~/components/providers/UserProvider";
import {
  GET_AUDIO_QUERY_BY_SLUG_KEY,
  GET_AUDIO_QUERY_KEY,
  GET_USER_AUDIOS_QUERY_KEY,
} from "~/lib/hooks/api/keys";

export function useCreateAudio(): UseMutationResult<
  AudioView,
  unknown,
  CreateAudioRequest,
  unknown
> {
  const queryClient = useQueryClient();
  const { user } = useUser();

  const mutationFn = async (input: CreateAudioRequest): Promise<AudioView> => {
    const { data } = await request<AudioView>({
      url: "audios",
      method: "post",
      data: input,
    });
    return data;
  };

  return useMutation(mutationFn, {
    onSuccess(data) {
      queryClient.setQueryData<AudioView>(GET_AUDIO_QUERY_KEY(data.id), data);
      queryClient.setQueryData<AudioView>(
        GET_AUDIO_QUERY_BY_SLUG_KEY(data.slug),
        data
      );
      if (user) {
        queryClient.invalidateQueries(GET_USER_AUDIOS_QUERY_KEY(user.userName));
      }
    },
  });
}
