import { RequestContext, getCookie, setCookie, ResponseContext } from "./cookies";

export function getAccessToken(ctx?: RequestContext) {
  return getCookie("accessToken", ctx);
}

export function setAccessTokenCookie(value: string, age: number, ctx?: ResponseContext) {
  setCookie("accessToken", value, ctx, {
    path: "/",
    sameSite: true,
    maxAge: age
  });
}

export function getRefreshToken(ctx?: RequestContext) {
  return getCookie("refreshToken", ctx);
}

export function setRefreshTokenCookie(value: string, expires: number, ctx?: ResponseContext) {
  setCookie("refreshToken", value, ctx, {
    expires: new Date(expires * 1000),
    path: '/',
    httpOnly: true,
    sameSite: true
  });
}