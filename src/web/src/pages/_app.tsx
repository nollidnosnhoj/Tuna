import React from "react";
import { ChakraProvider } from "@chakra-ui/react";
import { AppProps as NextAppProps } from "next/app";
import dynamic from "next/dynamic";
import { QueryClient, QueryClientProvider } from "react-query";
import { ReactQueryDevtools } from "react-query/devtools";
import PageLoader from "~/components/Page/PageLoader";
import { UserProvider } from "~/features/user/components/providers";
import { CurrentUser } from "~/features/user/types";
import theme from "~/lib/theme";
import queryClient from "~/lib/queryClient";
import LoginModal from "~/features/auth/components/AuthModal";

interface AppProps extends NextAppProps {
  user?: CurrentUser;
}

const AudioPlayer = dynamic(
  () => import("~/features/audio/components/AudioPlayer"),
  {
    ssr: false,
  }
);

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
            <AudioPlayer />
            <LoginModal />
          </UserProvider>
        </ChakraProvider>
      </QueryClientProvider>
    </>
  );
}

export default App;
