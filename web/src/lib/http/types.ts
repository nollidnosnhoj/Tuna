import { Method } from "axios";
import { GetServerSidePropsContext, NextPageContext } from "next";

export type ApiMethod = Method;
export type NextSSRContext = NextPageContext | GetServerSidePropsContext;

export interface ApiRequestConfig {
  route: string;
  method: ApiMethod;
  params?: {
    [key: string]:
      | string
      | string[]
      | number
      | number[]
      | boolean
      | boolean[]
      | undefined;
  };
  body?: any;
  skipAuthRefresh?: boolean;
  validateStatus?: (status: number) => boolean;
}

export const TOKEN_CONSTANTS = {
  ACCESS_TOKEN: "ac_accessToken",
  REFRESH_TOKEN: "ac_refreshToken",
  ACCESS_TOKEN_EXPIRES: "ac_at_expires",
};
