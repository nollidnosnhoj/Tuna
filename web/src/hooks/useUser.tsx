import { useContext } from "react";
import { UserContext } from "../contexts/userContext";

export default function useUser() {
  let context = useContext(UserContext);
  if (!context) throw new Error("Please implement UserProvider.");
  return context;
}
