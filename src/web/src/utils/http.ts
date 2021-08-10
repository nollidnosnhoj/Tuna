/* eslint-disable @typescript-eslint/explicit-module-boundary-types */
/* eslint-disable @typescript-eslint/no-explicit-any */
import { AxiosError } from "axios";

export function isAxiosError<T = any>(err: any): err is AxiosError<T> {
  if (!err) return false;
  return (err as AxiosError<T>).isAxiosError !== undefined;
}

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

export function stringifyQueryObject(queryObj: Record<string, any>): string {
  for (const key in queryObj) {
    // remove entries that has a value of undefined.
    if (typeof queryObj[key] === "undefined" || queryObj[key] === null) {
      delete queryObj[key];
      // turn any value that is not a string into a string
    } else if (typeof queryObj[key] !== "string") {
      queryObj[key] = JSON.stringify(queryObj[key]);
    }
  }

  if (!Object.keys(queryObj)) return "";

  return `${new URLSearchParams(queryObj).toString()}`;
}

export function stringifyUrl(
  url: string,
  queryObj: Record<string, any>
): string {
  return url + stringifyQueryObject(queryObj);
}
