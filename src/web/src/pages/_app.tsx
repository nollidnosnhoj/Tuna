import React from "react";
import { ChakraProvider } from "@chakra-ui/react";
import { AppProps as NextAppProps } from "next/app";
import { QueryClient, QueryClientProvider } from "react-query";
import { ReactQueryDevtools } from "react-query/devtools";
import PageLoader from "~/components/Page/Loader";
import { UserProvider } from "~/features/user/components/providers";
import { CurrentUser } from "~/features/user/api/types";
import theme from "~/lib/theme";
import queryClient from "~/lib/queryClient";
import AddToPlaylistModal from "~/features/playlist/components/Modals/AddToPlaylistModal";

interface AppProps extends NextAppProps {
  user?: CurrentUser;
}

function App({ Component, user, pageProps }: AppProps) {
  const queryClientRef = React.useRef<QueryClient>();
  if (!queryClientRef.current) {
    queryClientRef.current = queryClient;
  }

  return (
    <>
      <QueryClientProvider client={queryClientRef.current}>
        <ChakraProvider resetCSS theme={theme}>
          <UserProvider initialUser={user || null}>
            <ReactQueryDevtools initialIsOpen={false} />
            <PageLoader color={theme.colors.primary[500]} />
            <Component {...pageProps} />
            <AddToPlaylistModal />
          </UserProvider>
        </ChakraProvider>
      </QueryClientProvider>
    </>
  );
}

export default App;
