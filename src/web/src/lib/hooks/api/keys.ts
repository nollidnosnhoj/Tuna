import { QueryKey } from "react-query";
import { ID } from "~/lib/types";

export const GET_AUDIO_QUERY_BY_SLUG_KEY = (audioSlug: string): QueryKey => [
  "audios",
  audioSlug,
];
export const GET_AUDIO_QUERY_KEY = (audioId: ID): QueryKey => [
  "audios",
  audioId,
];
export const GET_AUDIO_FEED_QUERY_KEY: QueryKey = "feed";
export const GET_AUDIO_LIST_QUERY_KEY = "audios";
export const ME_QUERY_KEY = "me";
export const GET_PLAYLIST_KEY = (playlistId: ID): QueryKey => [
  "playlist",
  playlistId,
];
export const GET_PLAYLIST_AUDIOS_KEY = (
  playlistId: ID | undefined
): QueryKey => ["playlist_audios", playlistId];
export const GET_PROFILE_QUERY_KEY = (username: string): QueryKey => [
  "profile",
  username,
];
export const GET_TAG_AUDIO_LIST_QUERY_KEY = "audios";
export const GET_USER_AUDIOS_QUERY_KEY = (username: string): QueryKey => [
  "userAudios",
  username,
];
export const GET_USER_FAVORITE_AUDIOS_QUERY_KEY = (
  username: string
): QueryKey => ["userFavoriteAudios", username];
export const SEARCH_AUDIO_QUERY_KEY = (
  term: string,
  tags?: string
): QueryKey => ["searchAudios", { q: term, tags: tags }];
export const GET_YOUR_AUDIOS_KEY = "your_audios";
export const GET_YOUR_FAV_AUDIOS_KEY = "your_fav_audios";
export const GET_YOUR_PLAYLISTS_KEY = "your_playlists";