import { AxiosError } from "axios";

export function isAxiosError(err: any): err is AxiosError {
  return (err as AxiosError).isAxiosError !== undefined;
}