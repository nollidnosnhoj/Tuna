/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/explicit-module-boundary-types */
import { createStandaloneToast } from "@chakra-ui/react";
import theme from "~/lib/theme";
import { ErrorResponse } from "~/lib/types";
import { isAxiosError } from "./http";

const toast = createStandaloneToast({ theme: theme });

export function errorToast(options: {
  message: string;
  title?: string;
  duration?: number;
  isClosable?: boolean;
}): void {
  toast({
    title: options.title ?? "An error occurred.",
    description: options.message,
    status: "error",
    duration: options.duration ?? 5000,
    isClosable: options.isClosable ?? true,
  });
}

export function apiErrorToast(err: any): void {
  let message =
    "An error has occured while processing request. Please try again later.";
  const title = "API Error Occurred.";
  if (isAxiosError<ErrorResponse>(err)) {
    if (err?.response?.status === 401) return;
    message = err?.response?.data.message ?? message;
    errorToast({ title, message });
    if (err.response?.data.errors) {
      Object.entries(err.response.data.errors).forEach(([key, errors]) => {
        errorToast({
          title: key,
          message: errors.join(". "),
        });
      });
    }
    return;
  }
  errorToast({ title, message });
}

export function successfulToast(options: {
  title?: string;
  message?: string;
  duration?: number;
  isClosable?: boolean;
}): void {
  const title = "Success!";
  const message = "Your request has been successfully processed.";
  toast({
    title: options.title ?? title,
    description: options.message ?? message,
    status: "success",
    duration: options.duration ?? 5000,
    isClosable: options.isClosable ?? true,
  });
}
