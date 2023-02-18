/* eslint-disable @typescript-eslint/no-explicit-any */
import axios from "axios";
import {
  GetServerSidePropsContext,
  GetServerSidePropsResult,
  PreviewData,
} from "next";
import { ParsedUrlQuery } from "querystring";
import { isAxiosError } from "~/utils";
import { CurrentUser } from "../types";
import SETTINGS from "~/lib/config";

type AuthGetServerSideProps<
  P extends Record<string, any> = Record<string, any>,
  Q extends ParsedUrlQuery = ParsedUrlQuery,
  D extends PreviewData = PreviewData
> = (
  context: GetServerSidePropsContext<Q, D>,
  user: CurrentUser
) => Promise<GetServerSidePropsResult<P>>;

export const authRoute = <T extends Record<string, any> = Record<string, any>>(
  getServerSideProps: AuthGetServerSideProps<T>
) =>
  async function (
    context: GetServerSidePropsContext
  ): Promise<GetServerSidePropsResult<T>> {
    const { resolvedUrl, req } = context;
    try {
      const response = await axios.request<CurrentUser>({
        baseURL: SETTINGS.BACKEND_API,
        url: "me",
        method: "GET",
        headers: {
          ...(req ? { cookie: req.headers.cookie } : {}),
        },
      });

      return getServerSideProps(context, response.data);
    } catch (err) {
      if (isAxiosError(err) && err.response?.status === 401) {
        return {
          redirect: {
            destination: `/login?status=unauthorized${
              resolvedUrl ? "&redirecturl=" + encodeURI(resolvedUrl) : ""
            }`,
            statusCode: 307,
          },
        };
      }
      throw err;
    }
  };
