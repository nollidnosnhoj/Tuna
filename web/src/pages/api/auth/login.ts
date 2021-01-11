import type { NextApiRequest, NextApiResponse } from 'next'
import { AuthResult } from '~/lib/types';
import request from '~/lib/request';
import { isAxiosError } from '~/utils/axios';
import { setAccessTokenCookie, setRefreshTokenCookie } from '~/utils/cookies'

export type BackendAuthResult = {
  accessToken: string,
  refreshToken: string,
  refreshTokenExpires: number
}

export default async (req: NextApiRequest, res: NextApiResponse<AuthResult>) => {
  try {
    if (req.method?.toUpperCase() !== 'POST') {
      res.status(404);
      return;
    }

    const loginRequest = req.body;

    const { status, data } = await request<BackendAuthResult>('auth/login', {
      method: 'post',
      body: loginRequest,
      skipAuthRefresh: true
    });

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