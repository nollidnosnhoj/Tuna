import { Box } from "@chakra-ui/react";
import Head from "next/head";
import React from "react";
import Header from "~/components/Header";
import Container from "~/components/Container";

interface PageProps {
  title?: string;
  removeHeader?: boolean;
  removeSearchBar?: boolean;
}

const Page: React.FC<PageProps> = ({
  title = "Audiochan",
  removeHeader = false,
  removeSearchBar = false,
  children,
  ...props
}) => {
  return (
    <>
      <Head>
        <title>{title}</title>
      </Head>
      {!removeHeader && <Header removeSearchBar={removeSearchBar} />}
      <Container mt="120px" mb="100px" {...props}>
        {children}
      </Container>
    </>
  );
};

export default Page;
