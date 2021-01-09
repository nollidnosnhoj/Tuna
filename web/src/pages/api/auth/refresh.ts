import type { NextApiRequest, NextApiResponse } from 'next'
import { isAxiosError } from '~/utils/axios';
import { getRefreshToken, setAccessTokenCookie, setRefreshTokenCookie } from '~/utils/cookies'
import { BackendAuthResult } from './login';
import request from '~/lib/request';

export default async (req: NextApiRequest, res: NextApiResponse ) => {
  try {
    if (req.method?.toUpperCase() !== 'POST') {
      res.status(404).end();
      return;
    }

    const refreshToken = getRefreshToken({ req }) || "";
    
    const { status, data } = await request<BackendAuthResult>('auth/refresh', {
      method: 'post',
      body: { refreshToken: refreshToken },
      skipAuthRefresh: true
    });

    setAccessTokenCookie(data.accessToken, { res });
    setRefreshTokenCookie(data.refreshToken, data.refreshTokenExpires, { res });
    res.status(status).json({ accessToken: data.accessToken });
  } catch (err) {
    console.log(err);
    if (!isAxiosError(err)) {
      res.status(500);
    } else {
      const status = err.response.status;
      res.status(status).json(err.response.data);
    }
  }
}