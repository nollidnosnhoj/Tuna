import { Box } from "@chakra-ui/react";
import Head from "next/head";
import React from "react";
import Container from "~/components/Container";
import Header from "~/components/Header";

const PageLayout: React.FC<{
  title?: string;
  beforeContainer?: React.ReactNode;
}> = ({ title, beforeContainer, children, ...props }) => {
  return (
    <>
      <Head>
        <title>{title ?? "Audiochan"}</title>
      </Head>
      <Header title="Audiochan" />
      {beforeContainer}
      <Container {...props}>
        <Box paddingX="5" paddingY="1.5rem">
          {children}
        </Box>
      </Container>
    </>
  );
};

export default PageLayout;
