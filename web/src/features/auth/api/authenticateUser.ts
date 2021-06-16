import Axios from "axios";
import SETTINGS from "~/lib/config";
import { setAccessTokenExpiration } from "~/lib/http/utils";
import { LoginFormValues } from "../components/LoginForm";

export async function authenticateUser(
  request: LoginFormValues
): Promise<[string, number]> {
  const { data } = await Axios.request({
    method: "post",
    baseURL: SETTINGS.FRONTEND_URL,
    url: "api/auth/login",
    withCredentials: true,
    data: request,
  });
  setAccessTokenExpiration(data.accessTokenExpires);
  return [data.accessToken, data.accessTokenExpires];
}
