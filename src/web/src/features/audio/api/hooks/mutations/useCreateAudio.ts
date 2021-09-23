import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { GET_USER_AUDIOS_QUERY_KEY } from "~/features/user/api/hooks/queries/useGetUserAudios";
import { useUser } from "~/features/user/hooks";
import request from "~/lib/http";
import { AudioView, CreateAudioRequest } from "../../types";
import {
  GET_AUDIO_QUERY_BYSLUG_KEY,
  GET_AUDIO_QUERY_KEY,
} from "../queries/useGetAudio";

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
        GET_AUDIO_QUERY_BYSLUG_KEY(data.slug),
        data
      );
      if (user) {
        queryClient.invalidateQueries(GET_USER_AUDIOS_QUERY_KEY(user.userName));
      }
    },
  });
}
