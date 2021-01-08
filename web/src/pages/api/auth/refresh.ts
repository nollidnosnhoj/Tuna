import type { NextApiRequest, NextApiResponse } from 'next'
import axios from 'axios'
import CONSTANTS from '~/constants'
import { isAxiosError } from '~/utils/axios';
import { getCookie, getRefreshToken, setAccessTokenCookie, setRefreshTokenCookie } from '~/utils/cookies'
import { BackendAuthResult } from './login';

export default async (req: NextApiRequest, res: NextApiResponse ) => {
  try {
    if (req.method?.toUpperCase() !== 'POST') {
      res.status(404).end();
      return;
    }

    const refreshToken = getRefreshToken({ req }) || "";
    
    const response = await axios.post<BackendAuthResult>(CONSTANTS.API_URL + 'auth/refresh', JSON.stringify({ refreshToken: refreshToken }), { 
      headers: {
        'Content-Type': 'application/json'
      }
    });

    const { data, status } = response;

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