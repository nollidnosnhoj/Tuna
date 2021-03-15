import { Box } from "@chakra-ui/react";
import Head from "next/head";
import React from "react";
import Header from "~/components/Header";
import Container from "~/components/Container";

interface PageProps {
  title?: string;
  removeHeader?: boolean;
  removeSearchBar?: boolean;
  beforeContainer?: React.ReactNode;
}

const Page: React.FC<PageProps> = ({
  title = "Audiochan",
  removeHeader = false,
  removeSearchBar = false,
  beforeContainer,
  children,
  ...props
}) => {
  return (
    <>
      <Head>
        <title>{title}</title>
      </Head>
      <Box boxShadow="md">
        {!removeHeader && <Header removeSearchBar={removeSearchBar} />}
        {beforeContainer}
      </Box>
      <Container pb="120px" pt={8} {...props}>
        {children}
      </Container>
    </>
  );
};

export default Page;
