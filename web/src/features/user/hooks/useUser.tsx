import { useContext } from "react";
import { UserContext } from "../contexts";

export function useUser() {
  const context = useContext(UserContext);
  if (!context) throw new Error("Cannot find UserContext.");
  return context;
}
