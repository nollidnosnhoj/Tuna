export type Creator = {
  id: number;
  username: string;
  displayName: string;
}

export type AuthResult = {
  accessToken: string,
}

export type ErrorResponse = {
  title: string;
  message: string;
  errors?: { [key: string]: string[] }
}
