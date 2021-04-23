import { ParsedUrlQuery } from "querystring";
import slugify from "slugify";
import { v4 as uuidv4 } from 'uuid'
import { Audio, AudioDetail } from "~/features/audio/types";

export const validationMessages = {
  required: function (field: string) {
    return `${field} is required.`;
  },
  min: function (field: string, min: number) {
    return `${field} must be at least ${min} characters long.`;
  },
  max: function (field: string, max: number) {
    return `${field} must be no more than ${max} characters long.`;
  },
};

export function taggify(value: string) {
  return slugify(value, {
    replacement: '-',
    lower: true,
    strict: true
  });
}

export function objectToFormData(values: object): FormData {
  var formData = new FormData();

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