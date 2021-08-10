import { QueryKey, useMutation, useQuery, useQueryClient } from "react-query";
import { useUser } from "~/features/user/hooks";
import {
  checkIfPlaylistFavoritedRequest,
  favoriteAPlaylistRequest,
  unfavoriteAPlaylistRequest,
} from "..";

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

  const { data, isLoading: isQueryLoading } = useQuery(
    IS_FAVORITE_PLAYLIST_QUERY_KEY(playlistId),
    () => checkIfPlaylistFavoritedRequest(playlistId),
    {
      enabled: !!user,
      initialData: initialData,
    }
  );

  const { mutateAsync, isLoading: isMutationLoading } = useMutation<boolean>(
    () =>
      data
        ? unfavoriteAPlaylistRequest(playlistId)
        : favoriteAPlaylistRequest(playlistId),

    {
      onSuccess() {
        queryClient.setQueryData(
          IS_FAVORITE_PLAYLIST_QUERY_KEY(playlistId),
          !data
        );
      },
    }
  );

  return {
    isFavorite: data,
    favorite: mutateAsync,
    isLoading: isQueryLoading || isMutationLoading,
  };
}
