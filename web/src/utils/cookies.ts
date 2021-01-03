import { NextPageContext, NextApiRequest, NextApiResponse } from 'next'
import { CookieSerializeOptions, CookieParseOptions } from 'cookie';
import nookies from 'nookies'
import { ACCESS_TOKEN_KEY, REFRESH_TOKEN_KEY } from '~/constants';
import { IncomingMessage, ServerResponse } from 'http';

export type RequestContext = Pick<NextPageContext, 'req'> | { req: NextApiRequest }
export type ResponseContext = Pick<NextPageContext, 'res'> | { res: NextApiResponse }

export function allCookies(ctx?: RequestContext, options: CookieParseOptions = {}) {
  return nookies.get(ctx, options);
}

export function getCookie(key: string, ctx?: RequestContext, options: CookieParseOptions = {}) {
  const cookies = nookies.get(ctx, options);
  return cookies[key];
}

export function setCookie(key: string, value: string, ctx?: ResponseContext, options: CookieSerializeOptions = {}) {
  nookies.set(ctx, key, value, options);
}

export function removeCookie(key: string, ctx?: ResponseContext, options: CookieSerializeOptions = {}) {
  nookies.destroy(ctx, key, options);
}

export function setAccessTokenCookie(value: string, ctx?: ResponseContext) {
  setCookie(ACCESS_TOKEN_KEY, value, ctx, {
    path: "/",
    sameSite: true,
  });
}

export function setRefreshTokenCookie(value: string, expires: number, ctx?: ResponseContext) {
  console.log(expires * 1000 - Date.now())
  console.log(new Date(expires * 1000))
  setCookie(REFRESH_TOKEN_KEY, value, ctx, {
    expires: new Date(expires * 1000),
    path: '/',
    httpOnly: true,
    sameSite: true
  });
}

export default {
  cookies: allCookies,
  set: setCookie,
  get: getCookie,
  remove: removeCookie
}