import { QueryClient } from "react-query";
import { errorToast } from "~/utils/toast";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: false,
      staleTime: 60 * 1000,
      refetchOnWindowFocus: false,
      onError: (err) => {
        errorToast(err);
      },
    },
  },
});

export default queryClient;
