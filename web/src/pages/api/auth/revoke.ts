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
    await api.post('auth/revoke', body, {
      skipAuthRefresh: true
    });
  } catch (err) {
  } finally {
    setAccessTokenCookie('', 0, { res });
    setRefreshTokenCookie('', 0, { res })
    res.status(200).end();
  }
}