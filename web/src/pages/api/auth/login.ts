import type { NextApiRequest, NextApiResponse } from 'next'
import axios from 'axios'
import CONSTANTS from '~/constants'
import { AuthResultResponse } from '~/lib/types';
import { isAxiosError } from '~/utils/axios';
import { setAccessTokenCookie, setRefreshTokenCookie } from '~/utils/cookies'

export type BackendAuthResult = {
  accessToken: string,
  refreshToken: string,
  refreshTokenExpires: number
}

export default async (req: NextApiRequest, res: NextApiResponse<AuthResultResponse>) => {
  try {
    if (req.method?.toUpperCase() !== 'POST') {
      res.status(404);
      return;
    }

    const loginRequest = req.body;

    const response = await axios.post<BackendAuthResult>(CONSTANTS.API_URL + 'auth/login', loginRequest, {
      withCredentials: true
    });

    const { status, data } = response
    setAccessTokenCookie(data.accessToken, { res });
    setRefreshTokenCookie(data.refreshToken, data.refreshTokenExpires, { res });
    res.status(status).json({ accessToken: data.accessToken })
  } catch (err) {
    if (!isAxiosError(err)) {
      res.status(500);
    } else {
      const status = err.response?.status ?? 500;
      res.status(status).json(err.response.data);
    }
  }
}