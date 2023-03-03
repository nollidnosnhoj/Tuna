import { RelayEnvironmentProvider } from "react-relay";
import { RecordSource } from "relay-runtime";
import { useMemo, useEffect } from "react";
import { RelayPageProps } from "~/graphql/relay-types";
import { initRelayEnvironment } from "~/graphql/RelayEnvironment";
import type { AppProps } from "next/app";
import { MantineProvider } from "@mantine/core";

export default function App({
  Component,
  pageProps,
}: AppProps<RelayPageProps>) {
  const environment = useMemo(initRelayEnvironment, []);

  useEffect(() => {
    const store = environment.getStore();

    // Hydrate the store.
    store.publish(new RecordSource(pageProps.initialRecords));

    // Notify any existing subscribers.
    store.notify();
  }, [environment, pageProps.initialRecords]);

  return (
    <MantineProvider
      withGlobalStyles
      withNormalizeCSS
      theme={{
        /** Put your mantine theme override here */
        colorScheme: "light",
      }}
    >
      <RelayEnvironmentProvider environment={environment}>
        <Component {...pageProps} />
      </RelayEnvironmentProvider>
    </MantineProvider>
  );
}
