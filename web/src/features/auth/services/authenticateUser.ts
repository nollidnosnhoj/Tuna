import Axios from "axios";
import { LoginFormValues } from "../components/LoginForm";
import { AuthResult } from "../types";

export async function authenticateUser(
  request: LoginFormValues
): Promise<[string, number]> {
  const { data } = await Axios.post<AuthResult>("/api/auth/login", request, {
    withCredentials: true,
  });
  return [data.accessToken, data.accessTokenExpires];
}
