import {
  QueryKey,
  useQuery,
  UseQueryOptions,
  UseQueryResult,
} from "react-query";
import { IdSlug } from "~/lib/types";
import { getIdAndSlugFromSlug } from "~/utils";
import { getPlaylistRequest } from "..";
import { Playlist, PlaylistId } from "../types";

export const GET_PLAYLIST_KEY = (id: PlaylistId): QueryKey => ["playlist", id];

export function useGetPlaylist(
  slug: IdSlug,
  options: UseQueryOptions<Playlist> = {}
): UseQueryResult<Playlist> {
  const [id] = getIdAndSlugFromSlug(slug);
  return useQuery<Playlist>(
    GET_PLAYLIST_KEY(id),
    () => getPlaylistRequest(id),
    options
  );
}
