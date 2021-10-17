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

describe("objectToFormData", () => {
  it("Successfully transform JS Object into FormData", () => {
    const obj = {
      test1: "hello",
      test2: "world",
    };
    const fd = objectToFormData(obj);
    expect(fd.has("test1")).toBeTruthy();
    expect(fd.has("test2")).toBeTruthy();
    expect(fd.get("test1")).toBe("hello");
    expect(fd.get("test2")).toBe("world");
  });
});

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

describe("stringifyQueryObject", () => {
  it("Successfully stringify object", () => {
    const mock = {
      paramOne: "hello",
      paramTwo: true,
      paramThree: 10,
    };
    const expected = "paramOne=hello&paramTwo=true&paramThree=10";
    const result = stringifyQueryObject(mock);
    expect(result).toBe(expected);
  });
});

export function stringifyUrl(
  url: string,
  queryObj: Record<string, any>
): string {
  return url + stringifyQueryObject(queryObj);
}
