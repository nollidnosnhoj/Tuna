export type AuthResultResponse = {
  accessToken: string,
}

export type User = {
  id: number;
  username: string;
  roles?: string[];
  followingIds?: number[];
  favoriteAudioIds?: number[];
}