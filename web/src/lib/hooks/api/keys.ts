import { QueryKey } from "@tanstack/react-query";
import { ID } from "~/lib/types";

export const GET_AUDIO_QUERY_BY_SLUG_KEY = (audioSlug: string): QueryKey => [
  "audio",
  audioSlug,
];

export const GET_AUDIO_QUERY_KEY = (audioId: ID): QueryKey => [
  "audio",
  audioId,
];

export const GET_AUDIO_LIST_QUERY_KEY: QueryKey = ["audios"];
export const ME_QUERY_KEY: QueryKey = ["me"];
export const GET_PROFILE_QUERY_KEY = (username: string): QueryKey => [
  "profile",
  username,
];
export const GET_TAG_AUDIO_LIST_QUERY_KEY = (tag: string): QueryKey => [
  "audios",
  { tag },
];
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
export const GET_YOUR_AUDIOS_KEY: QueryKey = ["your_audios"];
export const GET_YOUR_FAV_AUDIOS_KEY: QueryKey = ["your_fav_audios"];
