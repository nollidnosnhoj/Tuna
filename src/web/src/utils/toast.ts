/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/explicit-module-boundary-types */
import { createStandaloneToast, UseToastOptions } from "@chakra-ui/react";
import theme from "~/lib/theme";
import { ErrorResponse } from "~/lib/types";
import { isAxiosError } from "./http";

const chakraToast = createStandaloneToast({ theme: theme });

type ToastStatus = "info" | "warning" | "success" | "error";

type ErrorToastOptions = Omit<
  UseToastOptions,
  "title" | "description" | "status" | "id"
>;

function checkIfErrorResponse(err: any): err is ErrorResponse {
  return "code" in err && "message" in err;
}

export function toast(
  status: ToastStatus,
  options: Omit<UseToastOptions, "status">
): void {
  const defaults: UseToastOptions = {
    duration: 5000,
    isClosable: true,
  };

  chakraToast({ ...defaults, ...options, status });
}

export function errorToast(
  err?: any,
  options: ErrorToastOptions = {
    duration: 5000,
    isClosable: true,
  }
): void {
  const toastOptions: UseToastOptions = {
    title: "An error has occurred.",
    description: "Please contact administrators.",
    status: "error",
    ...options,
  };

  if (isAxiosError(err)) {
    if (checkIfErrorResponse(err.response?.data)) {
      chakraToast({
        ...toastOptions,
        title: err.response?.data.message,
      });
      return;
    }
  }

  if (checkIfErrorResponse(err)) {
    chakraToast({
      ...toastOptions,
      title: err.message,
    });
    return;
  }

  if (err instanceof Error) {
    chakraToast({
      ...toastOptions,
    });
    return;
  }

  if (typeof err === "string") {
    chakraToast({ ...toastOptions, title: err });
    return;
  }

  chakraToast({
    ...toastOptions,
  });
}
