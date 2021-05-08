import type { NextApiRequest, NextApiResponse } from "next";
import api from "~/lib/api";
import {
  getRefreshToken,
  setAccessTokenCookie,
  setRefreshTokenCookie,
} from "~/utils";

export default async (
  req: NextApiRequest,
  res: NextApiResponse
): Promise<void> => {
  if (req.method?.toUpperCase() !== "POST") {
    res.status(404);
    return;
  }

  const refreshToken = getRefreshToken({ req });

  const body = { refreshToken: refreshToken };

  try {
    await api.post("auth/revoke", body, {
      skipAuthRefresh: true,
    });
  } catch (err) {
    console.log("error when revoking haha");
  } finally {
    setAccessTokenCookie("", 0, { res });
    setRefreshTokenCookie("", 0, { res });
    res.status(200).end();
  }
};
