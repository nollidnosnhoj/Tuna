import { createStandaloneToast } from '@chakra-ui/react'
import theme from '~/lib/theme'
import { isAxiosError } from './axios';

const toast = createStandaloneToast({ theme: theme });

export function errorToast(options: {
  message: string,
  title?: string,
  duration?: number,
  isClosable?: boolean
}) {
  toast({
    title: options.title ?? "An error occurred.",
    description: options.message,
    status: 'error',
    duration: options.duration ?? 5000,
    isClosable: options.isClosable ?? true
  });
}

export function apiErrorToast(err: any) {
  let message = "An error has occured while processing request. Please try again later."
  let title = "API Error Occurred."
  if (isAxiosError(err)) {
    if (err.response.status === 401) return;
    title = err.response.data.title ?? title
    message = err.response.data.message ?? message;
  } else {
    console.log(err);
  }
  errorToast({ title, message })
}

export function successfulToast(options: {
  title?: string,
  message?: string,
  duration?: number,
  isClosable?: boolean,
}) {
  let title = "Success!";
  let message = "Your request has been successfully processed.";
  toast({
    title: options.title ?? title,
    description: options.message ?? message,
    status: 'success',
    duration: options.duration ?? 5000,
    isClosable: options.isClosable ?? true
  })
}