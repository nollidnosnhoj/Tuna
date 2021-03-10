import { AxiosError } from "axios";

export function isAxiosError<T = any>(err: any): err is AxiosError<T> {
  if (!err) return false;
  return (err as AxiosError<T>).isAxiosError !== undefined;
}