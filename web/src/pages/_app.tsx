import React from "react";
import { ChakraProvider } from "@chakra-ui/react";
import { AppProps as NextAppProps } from "next/app";
import dynamic from "next/dynamic";
import { QueryClient, QueryClientProvider } from "react-query";
import { ReactQueryDevtools } from "react-query/devtools";
import { Hydrate } from "react-query/hydration";
import PageLoader from "~/components/PageLoader";
import { UserProvider } from "~/contexts/userContext";
import theme from "~/lib/theme";
import { CurrentUser } from "~/features/user/types";
import AudioPlayerProvider from "~/contexts/audioPlayerContext";
import "react-h5-audio-player/lib/styles.css";

interface AppProps extends NextAppProps {
  user?: CurrentUser;
}

const queryClientConfig = {
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
    },
  },
};

const AudioPlayer = dynamic(() => import("~/components/AudioPlayer"), {
  ssr: false,
});

function App({ Component, user, pageProps }: AppProps) {
  const queryClientRef = React.useRef<QueryClient>();
  if (!queryClientRef.current) {
    queryClientRef.current = new QueryClient(queryClientConfig);
  }

  return (
    <>
      <QueryClientProvider client={queryClientRef.current}>
        <Hydrate state={pageProps.dehydratedState}>
          <ChakraProvider resetCSS theme={theme}>
            <UserProvider initialUser={user}>
              <AudioPlayerProvider>
                <PageLoader color={theme.colors.primary[500]} />
                <Component {...pageProps} />
                <AudioPlayer />
              </AudioPlayerProvider>
            </UserProvider>
          </ChakraProvider>
        </Hydrate>
      </QueryClientProvider>
    </>
  );
}

export default App;
