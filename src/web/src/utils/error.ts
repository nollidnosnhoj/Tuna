/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/explicit-module-boundary-types */
import { ErrorResponse } from "~/lib/types";
import { isAxiosError } from ".";

export function checkIfErrorResponse(err: any): err is ErrorResponse {
  if (!err) return false;
  return "code" in err && "message" in err;
}

export function getErrorMessage(
  err: any,
  defaultMessage = "An error has occurred. Please contact administrators"
): string {
  if (checkIfErrorResponse(err)) {
    return err.message;
  }

  if (isAxiosError(err)) {
    if (checkIfErrorResponse(err.response?.data)) {
      return err.response?.data.message ?? defaultMessage;
    }
  }

  if (err instanceof Error) {
    return err.message;
  }

  if (typeof err === "string") {
    return err;
  }

  return defaultMessage;
}
