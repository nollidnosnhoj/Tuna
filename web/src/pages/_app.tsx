import React, { useState } from "react";
import { ChakraProvider } from "@chakra-ui/react";
import { AppProps as NextAppProps } from "next/app";
import {
  Hydrate,
  QueryClient,
  QueryClientProvider,
} from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import PageLoader from "~/components/Page/Loader";
import theme from "~/lib/theme";
import { CurrentUser } from "~/lib/types";
import { UserProvider } from "~/components/providers/UserProvider";
import AudioPlayer from "~/components/AudioPlayer";
import { errorToast } from "~/utils";
import ToastContainer from "~/utils/toast";

interface AppProps extends NextAppProps {
  user?: CurrentUser;
}

function App({ Component, user, pageProps }: AppProps) {
  const [queryClient] = useState(
    () =>
      new QueryClient({
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
      })
  );

  return (
    <>
      <QueryClientProvider client={queryClient}>
        <Hydrate state={pageProps.dehyrdatedState}>
          <ChakraProvider resetCSS theme={theme}>
            <UserProvider initialUser={user || null}>
              <ReactQueryDevtools initialIsOpen={false} />
              <PageLoader color={theme.colors.primary[500]} />
              <Component {...pageProps} />
              <AudioPlayer />
            </UserProvider>
            <ToastContainer />
          </ChakraProvider>
        </Hydrate>
      </QueryClientProvider>
    </>
  );
}

export default App;
