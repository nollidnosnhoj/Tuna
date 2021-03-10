import type { NextApiRequest, NextApiResponse } from 'next'
import api from '~/utils/api';
import { isAxiosError } from '~/utils/axios';
import { getRefreshToken, setAccessTokenCookie, setRefreshTokenCookie } from '~/utils/cookies'

export default async (req: NextApiRequest, res: NextApiResponse ) => {
  try {
    if (req.method?.toUpperCase() !== 'POST') {
      res.status(404).end();
      return;
    }

    const body = {
      refreshToken: getRefreshToken({ req }) || ""
    }
    
    const { status, data } = await api.post('auth/refresh', body, {
      skipAuthRefresh: true
    })

    setAccessTokenCookie(data.accessToken, { res });
    setRefreshTokenCookie(data.refreshToken, data.refreshTokenExpires, { res });
    res.status(status).json(data);
  } catch (err) {
    if (!isAxiosError(err)) {
      res.status(500).end();
    } else {
      const status = err?.response?.status || 500;
      res.status(status).json(err?.response?.data);
    }
  }
}