import React from "react";
import Head from "next/head";
import { Box } from "@chakra-ui/react";
import Header from "~/components/ui/Header";
import Container from "~/components/ui/Container";
import SearchBar from "./ui/SearchBar";
import UserSection from "./ui/UserSection";
import Logo from "./ui/Logo";

interface PageProps {
  title?: string;
  removeHeader?: boolean;
}

const Page: React.FC<PageProps> = ({
  title = "Audiochan",
  removeHeader = false,
  children,
  ...props
}) => {
  return (
    <Box>
      <Head>
        <title>{title}</title>
      </Head>
      {!removeHeader && (
        <Header
          logo={<Logo />}
          searchBar={<SearchBar />}
          userMenu={<UserSection />}
        />
      )}
      <Container mt="120px" mb="100px" {...props}>
        {children}
      </Container>
    </Box>
  );
};

export default Page;
