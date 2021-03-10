import { Box } from "@chakra-ui/react";
import Head from "next/head";
import React from "react";
import Header from "~/components/Header";
import Container from "~/components/Container";

interface PageProps {
  title?: string;
  useHeader?: boolean;
  beforeContainer?: React.ReactNode;
}

const Page: React.FC<PageProps> = ({
  title = "Audiochan",
  useHeader = true,
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
        {useHeader && <Header />}
        {beforeContainer}
      </Box>
      <Container pb="120px" pt={8} px="5" {...props}>
        {children}
      </Container>
    </>
  );
};

export default Page;
