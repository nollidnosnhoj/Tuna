import Axios from "axios";
import SETTINGS from "~/lib/config";
import { setAccessTokenExpiration } from "~/lib/http/utils";

export async function revokeRefreshToken(): Promise<void> {
  try {
    await Axios.request({
      method: "post",
      baseURL: SETTINGS.FRONTEND_URL,
      url: "api/auth/revoke",
      withCredentials: true,
      validateStatus: (s) => s < 500,
    });
  } finally {
    setAccessTokenExpiration(0);
  }
}
