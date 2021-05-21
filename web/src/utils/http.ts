/* eslint-disable @typescript-eslint/explicit-module-boundary-types */
/* eslint-disable @typescript-eslint/no-explicit-any */
import { AxiosError } from "axios";
import { ParsedUrlQuery } from "querystring";

export function isAxiosError<T = any>(err: any): err is AxiosError<T> {
  if (!err) return false;
  return (err as AxiosError<T>).isAxiosError !== undefined;
}

export function extractQueryParam<T = string | number | boolean>(
  param: string | string[] | undefined,
  defaultValue?: T
): T | undefined {
  if (typeof param === "undefined") return defaultValue;
  const pString = Array.isArray(param) ? param[0] : param;
  return JSON.parse(pString) as T;
}

export const extractPaginationInfoFromQuery = (
  query: ParsedUrlQuery
): { page?: number; size?: number } => {
  const page = extractQueryParam<number>(query["page"], 1);
  const size = extractQueryParam<number>(query["size"], 15);
  return { page, size };
};

export function objectToFormData(values: Record<string, any>): FormData {
  const formData = new FormData();

  Object.entries(values).forEach(([key, value]) => {
    if (value) {
      if (Array.isArray(value)) {
        value.forEach((val) => formData.append(key, val));
      } else if (value instanceof File) {
        formData.append(key, value);
      } else {
        formData.append(key, value.toString());
      }
    }
  });

  return formData;
}
