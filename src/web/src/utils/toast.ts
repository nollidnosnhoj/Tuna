/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/explicit-module-boundary-types */
import { createStandaloneToast, UseToastOptions } from "@chakra-ui/react";
import theme from "~/lib/theme";
import { isAxiosError } from "./http";

const chakraToast = createStandaloneToast({ theme: theme });

type ToastStatus = "info" | "warning" | "success" | "error";

type ErrorToastOptions = Omit<
  UseToastOptions,
  "title" | "description" | "status" | "id"
>;

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

  if (err instanceof Error) {
    chakraToast({
      ...toastOptions,
    });
  } else if (isAxiosError(err)) {
    if (err.response?.status === 401) return;
    if (err.response?.data.errors) {
      Object.entries(err.response?.data.errors).forEach(([key, errors]) => {
        chakraToast({
          ...toastOptions,
          title: key,
          description: (errors as string[]).join(". "),
        });
      });
    } else {
      chakraToast({
        ...toastOptions,
        title: "A request error has occurred.",
        description: err.response?.data.message,
      });
    }
  } else if (typeof err === "string") {
    chakraToast({
      ...toastOptions,
      description: err,
    });
  } else {
    chakraToast({
      ...toastOptions,
    });
  }
}
