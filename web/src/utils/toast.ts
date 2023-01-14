/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/explicit-module-boundary-types */
import { createStandaloneToast, UseToastOptions } from "@chakra-ui/react";
import theme from "~/lib/theme";
import { getErrorMessage } from "./error";

const { ToastContainer, toast: chakraToast } = createStandaloneToast({
  theme: theme,
});

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
  chakraToast({
    ...options,
    status: "error",
    description: getErrorMessage(err),
  });
}

export default ToastContainer;
