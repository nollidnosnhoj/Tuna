import type { NextApiRequest, NextApiResponse } from "next";
import {
  createApiAxiosInstance,
  getRefreshToken,
  removeAccessToken,
  removeRefreshToken,
} from "~/lib/http/utils";

export default async (
  req: NextApiRequest,
  res: NextApiResponse
): Promise<void> => {
  if (req.method?.toUpperCase() !== "POST") {
    res.status(404);
    return;
  }

  const instance = createApiAxiosInstance();

  try {
    await instance.request({
      url: "auth/revoke",
      method: "post",
      data: {
        refreshToken: getRefreshToken(req) || "",
      },
    });
  } finally {
    removeAccessToken(res);
    removeRefreshToken(res);
    res.status(200).end();
  }
};
