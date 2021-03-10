import type { NextApiRequest, NextApiResponse } from 'next'
import { isAxiosError } from '~/utils/axios';
import api from '~/utils/api';
import { getRefreshToken, setAccessTokenCookie, setRefreshTokenCookie } from '~/utils/cookies'

export default async (req: NextApiRequest, res: NextApiResponse) => {
  if (req.method?.toUpperCase() !== 'POST') {
    res.status(404)
    return;
  }

  const refreshToken = getRefreshToken({ req });

  const body = { refreshToken: refreshToken };

  try {
    const { status } = await api.post('auth/revoke', body, {
      skipAuthRefresh: true
    });

    setAccessTokenCookie('', { res });
    setRefreshTokenCookie('', 0, { res })

    res.status(status).end();
  } catch (err) {
    if (!isAxiosError(err)) {
      res.status(500).end();
    } else {
      const status = err?.response?.status || 500;
      res.status(status).json(err?.response?.data);
    }
  }
}