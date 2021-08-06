import { QueryKey } from "react-query";
import { AudioView } from "~/features/audio/api/types";
import {
  useInfinitePagination,
  UseInfinitePaginationOptions,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import { getPlaylistAudiosRequest } from "../api";

export const GET_PLAYLIST_AUDIOS_KEY = (id: string): QueryKey => [
  "playlist_audios",
  id,
];

export function useGetPlaylistAudios(
  id: string,
  options: UseInfinitePaginationOptions<AudioView> = {}
): UseInfinitePaginationReturnType<AudioView> {
  return useInfinitePagination<AudioView>(
    GET_PLAYLIST_AUDIOS_KEY(id),
    (page) => getPlaylistAudiosRequest(id, page),
    options
  );
}
