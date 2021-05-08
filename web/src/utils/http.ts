import { AxiosError } from "axios";
import { ParsedUrlQuery } from "querystring";

export function isAxiosError<T = any>(err: any): err is AxiosError<T> {
  if (!err) return false;
  return (err as AxiosError<T>).isAxiosError !== undefined;
}

export function extractQueryParam<T = string | number | boolean>(param: string | string[] | undefined, defaultValue?: T): T | undefined {
  if (typeof param === 'undefined') return defaultValue;
  let pString = Array.isArray(param) ? param[0] : param;
  return JSON.parse(pString) as T;
}

export const extractPaginationInfoFromQuery = (query: ParsedUrlQuery) => {
  const page = extractQueryParam<number>(query['page'], 1);
  const size = extractQueryParam<number>(query['size'], 15);
  return { page, size } 
}