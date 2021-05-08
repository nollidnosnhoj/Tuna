import {
  RequestContext,
  getCookie,
  setCookie,
  ResponseContext,
} from "./cookies";

export function getAccessToken(ctx?: RequestContext): string {
  return getCookie("accessToken", ctx);
}

export function setAccessTokenCookie(
  value: string,
  age: number,
  ctx?: ResponseContext
): void {
  setCookie("accessToken", value, ctx, {
    path: "/",
    sameSite: true,
    maxAge: age,
  });
}

export function getRefreshToken(ctx?: RequestContext): string {
  return getCookie("refreshToken", ctx);
}

export function setRefreshTokenCookie(
  value: string,
  expires: number,
  ctx?: ResponseContext
): void {
  setCookie("refreshToken", value, ctx, {
    expires: new Date(expires * 1000),
    path: "/",
    httpOnly: true,
    sameSite: true,
  });
}
