import { AxiosAuthRefreshRequestConfig } from "axios-auth-refresh";
import { IncomingMessage, ServerResponse } from "http";

export interface ApiRequestConfig
  extends Omit<AxiosAuthRefreshRequestConfig, "baseURL"> {
  res?: ServerResponse;
  req?: IncomingMessage;
}

export const TOKEN_CONSTANTS = {
  ACCESS_TOKEN: "ac_accessToken",
  REFRESH_TOKEN: "ac_refreshToken",
  ACCESS_TOKEN_EXPIRES: "ac_at_expires",
};
