import { QueryKey, useMutation, useQuery, useQueryClient } from "react-query";
import { GET_YOUR_FAV_AUDIOS_KEY } from "~/features/auth/hooks/useYourFavoriteAudios";
import { useUser } from "~/features/user/hooks";
import {
  favoriteAnAudioRequest,
  checkIfUserFavoritedAudioRequest,
  unfavoriteAnAudioRequest,
} from "../api";
import { AudioId } from "../types";

type UseFavoriteAudioResult = {
  isFavorite?: boolean;
  favorite: () => void;
  isLoading: boolean;
};

export const IS_FAVORITE_AUDIO_QUERY_KEY = (audioId: AudioId): QueryKey => [
  "isFavoriteAudio",
  audioId,
];

export function useFavoriteAudio(
  audioId: AudioId,
  initialData?: boolean
): UseFavoriteAudioResult {
  const { user } = useUser();
  const queryClient = useQueryClient();

  const { data, isLoading: isQueryLoading } = useQuery(
    IS_FAVORITE_AUDIO_QUERY_KEY(audioId),
    () => checkIfUserFavoritedAudioRequest(audioId),
    {
      enabled: !!user,
      initialData: initialData,
    }
  );

  const { mutateAsync, isLoading: isMutationLoading } = useMutation<boolean>(
    () =>
      data
        ? unfavoriteAnAudioRequest(audioId)
        : favoriteAnAudioRequest(audioId),
    {
      onSuccess() {
        queryClient.setQueryData(IS_FAVORITE_AUDIO_QUERY_KEY(audioId), !data);
        queryClient.invalidateQueries(GET_YOUR_FAV_AUDIOS_KEY);
      },
    }
  );

  return {
    isFavorite: data,
    favorite: mutateAsync,
    isLoading: isQueryLoading || isMutationLoading,
  };
}
