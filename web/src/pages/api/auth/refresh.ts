import type { NextApiRequest, NextApiResponse } from 'next'
import axios from 'axios'
import { REFRESH_TOKEN_KEY } from '~/constants';
import ENVIRONMENT from '~/constants/environment'
import { isAxiosError } from '~/utils/axios';
import { getCookie, setAccessTokenCookie, setRefreshTokenCookie } from '~/utils/cookies'
import { BackendAuthResult } from './login';

export default async (req: NextApiRequest, res: NextApiResponse ) => {
  try {
    if (req.method?.toUpperCase() !== 'POST') {
      res.status(404).end();
      return;
    }

    const refreshToken = getCookie(REFRESH_TOKEN_KEY, { req }) || "";
    
    const response = await axios.post<BackendAuthResult>(ENVIRONMENT.API_URL + 'auth/refresh', JSON.stringify({ refreshToken: refreshToken }), { 
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