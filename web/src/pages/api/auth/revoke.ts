import type { NextApiRequest, NextApiResponse } from "next";
import {
  createBareApiAxiosInstance,
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

  const instance = createBareApiAxiosInstance();

  try {
    await instance.request({
      url: "auth/revoke",
      method: "post",
      data: {
        refreshToken: getRefreshToken(req) || "",
      },
    });
  } catch (err) {
    console.log("error when revoking haha");
  } finally {
    removeAccessToken(res);
    removeRefreshToken(res);
    res.status(200).end();
  }
};
