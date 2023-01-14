import {
  useMutation,
  UseMutationResult,
  useQueryClient,
} from "@tanstack/react-query";
import request from "~/lib/http";
import { Audio, CreateAudioRequest } from "~/lib/types";
import { useUser } from "~/components/providers/UserProvider";
import {
  GET_AUDIO_QUERY_BY_SLUG_KEY,
  GET_AUDIO_QUERY_KEY,
  GET_USER_AUDIOS_QUERY_KEY,
} from "~/lib/hooks/api/keys";

export function useCreateAudio(): UseMutationResult<
  Audio,
  unknown,
  CreateAudioRequest,
  unknown
> {
  const queryClient = useQueryClient();
  const { user } = useUser();

  const mutationFn = async (input: CreateAudioRequest): Promise<Audio> => {
    const { data } = await request<Audio>({
      url: "audios",
      method: "post",
      data: input,
    });
    return data;
  };

  return useMutation(mutationFn, {
    onSuccess(data) {
      queryClient.setQueryData<Audio>(GET_AUDIO_QUERY_KEY(data.id), data);
      queryClient.setQueryData<Audio>(
        GET_AUDIO_QUERY_BY_SLUG_KEY(data.slug),
        data
      );
      if (user) {
        queryClient.invalidateQueries(GET_USER_AUDIOS_QUERY_KEY(user.userName));
      }
    },
  });
}
