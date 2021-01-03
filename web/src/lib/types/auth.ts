export type LoginFormValues = {
  username: string;
  password: string;
}

export type RegistrationFormValues = {
  username: string;
  email: string;
  password: string;
}

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