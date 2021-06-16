import type { NextApiRequest, NextApiResponse } from "next";
import {
  createApiAxiosInstance,
  setAccessToken,
  setRefreshToken,
} from "~/lib/http/utils";
import { isAxiosError } from "~/utils";

export type BackendAuthResult = {
  accessToken: string;
  refreshToken: string;
  refreshTokenExpires: number;
};

export default async (
  req: NextApiRequest,
  res: NextApiResponse
): Promise<void> => {
  try {
    if (req.method?.toUpperCase() !== "POST") {
      res.status(404).end();
    }

    const instance = createApiAxiosInstance();

    const { status, data } = await instance.request({
      url: "auth/login",
      method: "post",
      data: req.body,
    });

    setAccessToken(data.accessToken, res);
    setRefreshToken(data.refreshToken, data.refreshTokenExpires, res);
    res.status(status).json(data);
  } catch (err) {
    if (!isAxiosError(err)) {
      res.status(500).end();
    } else {
      const status = err.response?.status ?? 500;
      const data = err.response?.data;
      if (data) {
        res.status(status).json(err?.response?.data);
      } else {
        res.status(status).end();
      }
    }
  }
};
