import { NextPageContext, NextApiRequest, NextApiResponse } from 'next'
import { CookieSerializeOptions, CookieParseOptions } from 'cookie';
import nookies from 'nookies'

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

export default {
  cookies: allCookies,
  set: setCookie,
  get: getCookie,
  remove: removeCookie
}