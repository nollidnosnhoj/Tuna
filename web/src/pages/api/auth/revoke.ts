import type { NextApiRequest, NextApiResponse } from 'next'
import axios from 'axios'
import { REFRESH_TOKEN_KEY } from '~/constants';
import ENVIRONMENT from '~/constants/environment'
import { isAxiosError } from '~/utils/axios';
import cookieHelper, { setAccessTokenCookie, setRefreshTokenCookie } from '~/utils/cookies'

export default async (req: NextApiRequest, res: NextApiResponse) => {
  if (req.method?.toUpperCase() !== 'POST') {
    res.status(404)
    return;
  }

  const refreshToken = cookieHelper.get(REFRESH_TOKEN_KEY, { req });

  try {
    const { status } = await axios.post(ENVIRONMENT.API_URL + 'auth/revoke', { 
      refreshToken: refreshToken 
    }, { withCredentials: true });

    setAccessTokenCookie('', { res });
    setRefreshTokenCookie('', 0, { res })

    res.status(status).end();
  } catch (err) {
    if (!isAxiosError(err)) {
      res.status(500);
    } else {
      const status = err.response.status;
      res.status(status).json(err.response.data);
    }
  }
}