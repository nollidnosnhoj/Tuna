import { QueryKey, useMutation, useQuery, useQueryClient } from "react-query";
import { useAuth } from "~/features/auth/hooks/useAuth";
import { useUser } from "~/features/user/hooks/useUser";
import api from "~/lib/api";
import { isAxiosError } from "~/utils";

type UseFavoriteAudioResult = {
  isFavorite?: boolean;
  favorite: () => void;
  isLoading: boolean;
};

export const IS_FAVORITE_AUDIO_QUERY_KEY = (audioId: string): QueryKey => [
  "isFavoriteAudio",
  audioId,
];

export function useFavoriteAudio(
  audioId: string,
  initialData?: boolean
): UseFavoriteAudioResult {
  const { user } = useUser();
  const { accessToken } = useAuth();
  const queryClient = useQueryClient();

  const { data, isLoading: isQueryLoading } = useQuery(
    IS_FAVORITE_AUDIO_QUERY_KEY(audioId),
    async () => {
      try {
        await api.head(`me/favorites/audio/${audioId}`, {
          accessToken,
        });
        return true;
      } catch (err) {
        if (!isAxiosError(err)) {
          console.log(err);
        }
        return false;
      }
    },
    {
      enabled: !!user,
      initialData: initialData,
    }
  );

  const favoriteAudioHandler = async (): Promise<boolean> => {
    const method = data ? "DELETE" : "PUT";
    const response = await api.request(
      method,
      `me/favorites/audio/${audioId}`,
      {
        accessToken,
      }
    );
    return response.status === 200 ? true : false;
  };

  const { mutateAsync, isLoading: isMutationLoading } = useMutation<boolean>(
    favoriteAudioHandler,
    {
      onSuccess(data) {
        queryClient.setQueryData(IS_FAVORITE_AUDIO_QUERY_KEY(audioId), data);
      },
    }
  );

  return {
    isFavorite: data,
    favorite: mutateAsync,
    isLoading: isQueryLoading || isMutationLoading,
  };
}
