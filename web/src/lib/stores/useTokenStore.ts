import create from "zustand";
import { parseCookies } from "nookies";
import { TOKEN_CONSTANTS } from "../http/types";
import { getAccessTokenExpirationFromLocalStorage } from "../http/utils";

type UseTokenStoreType = {
  accessToken: string;
  accessTokenExpires: number;
  setAccessToken: (newToken: string) => void;
  setAccessTokenExpires: (expires: number) => void;
};

const getDefaults = (): {
  accessToken: string;
  accessTokenExpires: number;
} => {
  try {
    return {
      accessToken: parseCookies()[TOKEN_CONSTANTS.ACCESS_TOKEN] || "",
      accessTokenExpires: getAccessTokenExpirationFromLocalStorage(),
    };
  } catch {
    return {
      accessToken: "",
      accessTokenExpires: 0,
    };
  }
};

export const useTokenStore = create<UseTokenStoreType>((set) => ({
  ...getDefaults(),
  setAccessToken: (newToken) =>
    set((state) => ({
      ...state,
      accessToken: newToken,
    })),
  setAccessTokenExpires: (expires) =>
    set((state) => ({
      ...state,
      accessTokenExpires: expires,
    })),
}));
