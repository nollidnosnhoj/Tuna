import { createContext, useContext } from "react";
import { CurrentUser } from "../../features/user/types";

export type UserContextType = {
  user: CurrentUser | null;
  updateUser: (updatedUser: CurrentUser | null) => void;
};

export const UserContext = createContext<UserContextType>(
  {} as UserContextType
);

export const useUser = () => {
  return useContext(UserContext);
};
