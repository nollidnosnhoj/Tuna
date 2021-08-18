import { useCallback } from "react";
import { QueryKey, useMutation, useQuery, useQueryClient } from "react-query";
import { useUser } from "~/features/user/hooks";
import request from "~/lib/http";

type UseFavoriteAudioResult = {
  isFavorite?: boolean;
  favorite: () => void;
  isLoading: boolean;
};

export const IS_FAVORITE_PLAYLIST_QUERY_KEY = (
  playlistId: string
): QueryKey => ["isFavoritePlaylist", playlistId];

export function useFavoritePlaylist(
  playlistId: string,
  initialData?: boolean
): UseFavoriteAudioResult {
  const { user } = useUser();
  const queryClient = useQueryClient();

  const { data: isFavorite, isLoading: isQueryLoading } = useQuery(
    IS_FAVORITE_PLAYLIST_QUERY_KEY(playlistId),
    async (): Promise<boolean> => {
      try {
        const res = await request({
          method: "head",
          url: `me/favorites/playlists/${playlistId}`,
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
      enabled: !!user,
      initialData: initialData,
    }
  );

  const mutate = useCallback(async () => {
    if (isFavorite) {
      await request({
        method: "DELETE",
        url: `me/favorites/playlists/${playlistId}`,
      });
    } else {
      await request({
        method: "PUT",
        url: `me/favorites/playlists/${playlistId}`,
      });
    }
    return true;
  }, [isFavorite, playlistId]);

  const { mutateAsync, isLoading: isMutationLoading } = useMutation<boolean>(
    mutate,
    {
      onSuccess() {
        queryClient.setQueryData(
          IS_FAVORITE_PLAYLIST_QUERY_KEY(playlistId),
          !isFavorite
        );
      },
    }
  );

  return {
    isFavorite,
    favorite: mutateAsync,
    isLoading: isQueryLoading || isMutationLoading,
  };
}
