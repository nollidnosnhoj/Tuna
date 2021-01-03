import type { NextApiRequest, NextApiResponse } from 'next'
import { AuthResultResponse } from '~/lib/types';
import { isAxiosError } from '~/utils';
import { setAccessTokenCookie, setRefreshTokenCookie } from '~/utils/cookies'
import request from '~/lib/request';

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

    const response = await request<BackendAuthResult>('auth/login', { 
      method: 'post',
      body: loginRequest,
      skipAuthRefresh: true,
      ctx: { req }
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