import { createContext, useContext } from "react";
import { LoginFormValues } from "~/features/auth/components/LoginForm";

export interface AuthContextProviderProps {
  accessToken: string;
  isLoadingAuth: boolean;
  isLoggedIn: boolean;
  login: (inputs: LoginFormValues) => Promise<void>;
  logout: () => Promise<void>;
}

export const ACCESS_TOKEN_EXPIRATION_KEY = "expires";

export const AuthContext = createContext<AuthContextProviderProps>(
  {} as AuthContextProviderProps
);

export const useAuth = () => {
  return useContext(AuthContext);
};
