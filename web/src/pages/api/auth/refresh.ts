import type { NextApiRequest, NextApiResponse } from "next";
import {
  createBareApiAxiosInstance,
  getRefreshToken,
  removeAccessToken,
  removeRefreshToken,
  setAccessToken,
  setRefreshToken,
} from "~/lib/http/utils";
import { isAxiosError } from "~/utils";

export default async (
  req: NextApiRequest,
  res: NextApiResponse
): Promise<void> => {
  try {
    if (req.method?.toUpperCase() !== "POST") {
      res.status(404).end();
      return;
    }

    const instance = createBareApiAxiosInstance();

    const { status, data } = await instance.request({
      url: "auth/refresh",
      method: "post",
      data: {
        refreshToken: getRefreshToken(req) || "",
      },
    });

    setAccessToken(data.accessToken, res);
    setRefreshToken(data.refreshToken, data.refreshTokenExpires, res);
    res.status(status).json(data);
  } catch (err) {
    removeAccessToken(res);
    removeRefreshToken(res);
    const status = err?.response?.status || 500;
    if (isAxiosError(err) && err.response?.data) {
      res.status(status).json(err.response?.data);
    } else {
      res.status(status).end();
    }
  }
};
