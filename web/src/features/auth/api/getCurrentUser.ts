import { CurrentUser } from "~/features/user/types";
import request from "~/lib/http";

export async function getCurrentUser(): Promise<CurrentUser> {
  const response = await request<CurrentUser>({
    method: "get",
    url: "me",
  });
  return response.data;
}
