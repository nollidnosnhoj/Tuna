import { AxiosRequestConfig } from "axios";
import { IncomingMessage, ServerResponse } from "http";

export interface ApiRequestConfig extends Omit<AxiosRequestConfig, "baseURL"> {
  res?: ServerResponse;
  req?: IncomingMessage;
}
