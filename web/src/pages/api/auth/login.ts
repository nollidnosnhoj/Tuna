import type { NextApiRequest, NextApiResponse } from 'next'
import { AuthResult } from '~/features/auth/types';
import api from '~/utils/api';
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
      res.status(404).end();
    }

    const { status, data } = await api.post('auth/login', req.body, {
      skipAuthRefresh: true
    })

    setAccessTokenCookie(data.accessToken, { res });
    setRefreshTokenCookie(data.refreshToken, data.refreshTokenExpires, { res });
    res.status(status).json(data)
  } catch (err) {
    if (!isAxiosError(err)) {
      res.status(500).end();
    } else {
      const status = err.response?.status ?? 500;
      res.status(status).json(err?.response?.data);
    }
  }
}