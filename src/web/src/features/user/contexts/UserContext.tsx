import { createContext } from "react";
import { CurrentUser } from "../types";

export type UserContextType = {
  isLoadingUser: boolean;
  isLoggedIn: boolean;
  user: CurrentUser | null;
  updateUser: (updatedUser: CurrentUser | null) => void;
  refreshUser: () => Promise<void>;
};

export const UserContext = createContext<UserContextType>(
  {} as UserContextType
);
