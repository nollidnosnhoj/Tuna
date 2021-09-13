import { useCallback } from "react";
import { QueryKey, useMutation, useQuery, useQueryClient } from "react-query";
import { GET_YOUR_FAV_AUDIOS_KEY } from "~/features/auth/api/hooks/useYourFavoriteAudios";
import { useUser } from "~/features/user/hooks";
import request from "~/lib/http";
import { ID } from "~/lib/types";
import { errorToast } from "~/utils";

type UseFavoriteAudioResult = {
  isFavorite?: boolean;
  favorite: () => void;
  isLoading: boolean;
};

export const IS_FAVORITE_AUDIO_QUERY_KEY = (audioId: ID): QueryKey => [
  "isFavoriteAudio",
  audioId,
];

export function useFavoriteAudio(
  audioId: ID,
  initialData?: boolean
): UseFavoriteAudioResult {
  const { user } = useUser();
  const queryClient = useQueryClient();

  const { data: isFavorite, isLoading: isQueryLoading } = useQuery(
    IS_FAVORITE_AUDIO_QUERY_KEY(audioId),
    async () => {
      try {
        const res = await request({
          method: "head",
          url: `me/favorites/audios/${audioId}`,
          validateStatus: (status) => {
            return status === 404 || status < 400;
          },
        });

        return res.status !== 404;
      } catch (err) {
        return false;
      }
    },
    {
      enabled: !!user && !initialData,
      initialData: initialData,
      onError(err) {
        errorToast(err);
      },
    }
  );

  const mutateHandler = useCallback(async () => {
    if (isFavorite) {
      await request({
        method: "DELETE",
        url: `me/favorites/audios/${audioId}`,
      });
    } else {
      await request({
        method: "PUT",
        url: `me/favorites/audios/${audioId}`,
      });
    }
    return true;
  }, [audioId, isFavorite]);

  const { mutateAsync, isLoading: isMutationLoading } = useMutation<boolean>(
    mutateHandler,
    {
      onSuccess() {
        queryClient.setQueryData(
          IS_FAVORITE_AUDIO_QUERY_KEY(audioId),
          !isFavorite
        );
        queryClient.invalidateQueries(GET_YOUR_FAV_AUDIOS_KEY);
      },
    }
  );

  return {
    isFavorite,
    favorite: mutateAsync,
    isLoading: isQueryLoading || isMutationLoading,
  };
}
