import { QueryClient } from "react-query";
import { isAxiosError } from "~/utils/axios";
import { apiErrorToast } from "~/utils/toast";

const queryClient = new QueryClient({
  defaultOptions: {
    mutations: {
      onError: (err) => {
        if (isAxiosError(err)) {
          apiErrorToast(err)
          return;
        }
        console.log(err);
      }
    },
    queries: {
      retry: false,
      staleTime: 60 * 1000,
      refetchOnWindowFocus: false,
      onError: (err) => {
        if (isAxiosError(err)) {
          apiErrorToast(err)
          return;
        }
        console.log(err);
      }
    },
  },
})

export default queryClient;