import { QueryKey } from "react-query";
import { AudioView } from "~/features/audio/api/types";
import {
  useInfinitePagination,
  UseInfinitePaginationOptions,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import { getPlaylistAudiosRequest } from "../api";
import { PlaylistId } from "../api/types";

export const GET_PLAYLIST_AUDIOS_KEY = (id: PlaylistId): QueryKey => [
  "playlist_audios",
  id,
];

export function useGetPlaylistAudios(
  id: PlaylistId,
  options: UseInfinitePaginationOptions<AudioView> = {}
): UseInfinitePaginationReturnType<AudioView> {
  return useInfinitePagination<AudioView>(
    GET_PLAYLIST_AUDIOS_KEY(id),
    (page) => getPlaylistAudiosRequest(id, page),
    options
  );
}
