import { useContext } from "react";
import { UserContext } from "../contexts";
import { CurrentUser } from "../types";

type CurrentUserOrNull = CurrentUser | null;

export function useUser(): [
  CurrentUserOrNull,
  (user: CurrentUserOrNull | null) => void
] {
  const context = useContext(UserContext);
  if (!context) throw new Error("Cannot find UserContext.");
  return [context.user, context.updateUser];
}
