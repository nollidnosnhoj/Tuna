import { useContext } from "react";
import { UserContext } from "~/contexts/UserContext";

export default function useUser() {
  const context = useContext(UserContext);
  if (!context) throw new Error("Cannot find UserContext.");
  return context;
}