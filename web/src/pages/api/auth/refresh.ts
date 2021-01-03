import type { NextApiRequest, NextApiResponse } from 'next'
import { REFRESH_TOKEN_KEY } from '~/constants';
import { AuthResultResponse } from '~/lib/types';
import request from '~/lib/request'
import { isAxiosError } from '~/utils';
import { getCookie, setAccessTokenCookie, setRefreshTokenCookie } from '~/utils/cookies'
import { BackendAuthResult } from './login';

export default async (req: NextApiRequest, res: NextApiResponse ) => {
  try {
    if (req.method?.toUpperCase() !== 'POST') {
      res.status(404).end();
      return;
    }

    const refreshToken = getCookie(REFRESH_TOKEN_KEY, { req }) || "";
    
    const response = await request<BackendAuthResult>('auth/refresh', {
      method: 'post',
      body: { refreshToken: refreshToken },
      skipAuthRefresh: true,
      ctx: { req }
    });

    const { data, status } = response;

    setAccessTokenCookie(data.accessToken, { res });
    setRefreshTokenCookie(data.refreshToken, data.refreshTokenExpires, { res });
    res.status(status).json({ accessToken: data.accessToken });
  } catch (err) {
    if (!isAxiosError(err)) {
      res.status(500);
    } else {
      const status = err.response.status;
      res.status(status).json(err.response.data);
    }
  }
}