import type { NextApiRequest, NextApiResponse } from 'next'
import axios from 'axios'
import CONSTANTS from '~/constants'
import { isAxiosError } from '~/utils/axios';
import cookieHelper, { getRefreshToken, setAccessTokenCookie, setRefreshTokenCookie } from '~/utils/cookies'

export default async (req: NextApiRequest, res: NextApiResponse) => {
  if (req.method?.toUpperCase() !== 'POST') {
    res.status(404)
    return;
  }

  const refreshToken = getRefreshToken({ req });

  try {
    const { status } = await axios.post(CONSTANTS.API_URL + 'auth/revoke', { 
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