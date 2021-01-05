export type AuthResultResponse = {
  accessToken: string,
}

export type User = {
  id: number;
  username: string;
  email: string;
  roles?: string[];
}