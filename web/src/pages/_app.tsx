import { ChakraProvider } from "@chakra-ui/react";
import { AppProps as NextAppProps } from "next/app";
import PageLoader from "~/components/PageLoader";
import { UserProvider } from "~/lib/contexts/user_context";
import theme from "~/lib/theme";
import fetcher from "~/lib/fetcher";
import { SWRConfig } from "swr";
import { AudioPlayerProvider } from "~/lib/contexts/audio_player_context";

interface AppProps extends NextAppProps {}

function App({ Component, pageProps }: AppProps) {
  return (
    <SWRConfig
      value={{
        revalidateOnFocus: false,
        fetcher: fetcher,
      }}
    >
      <ChakraProvider resetCSS theme={theme}>
        <UserProvider>
          <AudioPlayerProvider>
            <PageLoader color={theme.colors.primary[500]} />
            <Component {...pageProps} />
          </AudioPlayerProvider>
        </UserProvider>
      </ChakraProvider>
    </SWRConfig>
  );
}

export default App;
