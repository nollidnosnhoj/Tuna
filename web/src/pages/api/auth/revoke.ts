import type { NextApiRequest, NextApiResponse } from 'next'
import { ACCESS_TOKEN_KEY, REFRESH_TOKEN_KEY } from '~/constants';
import request from '~/lib/request';
import { isAxiosError } from '~/utils';
import cookieHelper, { setAccessTokenCookie, setRefreshTokenCookie } from '~/utils/cookies'

export default async (req: NextApiRequest, res: NextApiResponse) => {
  if (req.method?.toUpperCase() !== 'POST') {
    res.status(404)
    return;
  }

  const refreshToken = cookieHelper.get(REFRESH_TOKEN_KEY, { req });

  try {
    const { status } = await request('auth/revoke', {
      method: 'post',
      body: { refreshToken },
      skipAuthRefresh: true,
      ctx: { req }
    });

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