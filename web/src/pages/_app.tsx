import { ChakraProvider } from "@chakra-ui/react";
import { AppProps as NextAppProps } from "next/app";
import { QueryClientProvider, QueryClient } from "react-query";
import { Hydrate } from "react-query/hydration";
import { ReactQueryDevtools } from "react-query/devtools";
import PageLoader from "~/components/Shared/PageLoader";
import { UserProvider } from "~/lib/contexts/user_context";
import theme from "~/lib/theme";
import { AudioPlayerProvider } from "~/lib/contexts/audio_player_context";
import { CurrentUser } from "~/lib/types/user";

interface AppProps extends NextAppProps {
  user?: CurrentUser;
}

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
    },
  },
});

function App({ Component, user, pageProps }: AppProps) {
  return (
    <QueryClientProvider client={queryClient}>
      <Hydrate state={pageProps.dehydratedState}>
        <ChakraProvider resetCSS theme={theme}>
          <UserProvider initialUser={user}>
            <AudioPlayerProvider>
              <PageLoader color={theme.colors.primary[500]} />
              <Component {...pageProps} />
            </AudioPlayerProvider>
          </UserProvider>
        </ChakraProvider>
        <ReactQueryDevtools initialIsOpen={false} />
      </Hydrate>
    </QueryClientProvider>
  );
}

export default App;
