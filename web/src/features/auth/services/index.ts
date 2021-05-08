import Axios from "axios";
import { LoginFormValues } from "~/features/auth/components/LoginForm";
import { AuthResult } from "../types";

export async function authenticateUser(
  request: LoginFormValues
): Promise<[string, number]> {
  const { data } = await Axios.post<AuthResult>("/api/auth/login", request, {
    withCredentials: true,
  });
  return [data.accessToken, data.accessTokenExpires];
}

export async function refreshAccessToken(): Promise<[string, number]> {
  const { data } = await Axios.post<AuthResult>(
    "/api/auth/refresh",
    undefined,
    {
      withCredentials: true,
    }
  );
  return [data.accessToken, data.accessTokenExpires];
}

export async function revokeRefreshToken(): Promise<void> {
  await Axios.post("/api/auth/revoke", undefined, {
    withCredentials: true,
    validateStatus: (status) => status < 500,
  });
}
