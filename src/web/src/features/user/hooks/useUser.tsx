import { useContext } from "react";
import { UserContext, UserContextType } from "../contexts";

export function useUser(): UserContextType {
  const context = useContext(UserContext);
  if (!context) throw new Error("Cannot find UserContext.");
  return context;
}
