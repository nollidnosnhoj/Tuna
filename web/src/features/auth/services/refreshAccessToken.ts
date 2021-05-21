import Axios from "axios";
import { AuthResult } from "../types";

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
