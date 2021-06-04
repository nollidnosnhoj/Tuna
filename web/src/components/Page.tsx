import React from "react";
import Head from "next/head";
import NextLink from "next/link";
import { Box, BoxProps, Button, Flex, Heading } from "@chakra-ui/react";
import Header from "~/components/ui/Header";
import Container from "~/components/ui/Container";
import SearchBar from "./ui/SearchBar";
import UserSection from "./ui/UserSection";
import Logo from "./ui/Logo";
import { useAuth } from "~/features/auth/hooks";

const PageContainer: React.FC<BoxProps> = ({ children, ...props }) => (
  <Box>
    <Header
      logo={<Logo />}
      searchBar={<SearchBar />}
      userMenu={<UserSection />}
    />
    <Container mt="120px" mb="100px" {...props}>
      {children}
    </Container>
  </Box>
);

interface PageProps {
  title?: string;
  requiresAuth?: boolean;
}

const Page: React.FC<PageProps> = ({
  title = "Audiochan",
  requiresAuth = false,
  children,
  ...props
}) => {
  const { isLoggedIn } = useAuth();

  if (!isLoggedIn && requiresAuth) {
    return (
      <>
        <Head>
          <title>Unauthorized</title>
        </Head>
        <PageContainer {...props}>
          <Flex justify="center" align="center" height="50vh">
            <Box>
              <Heading as="h2">You are not authorized.</Heading>
              <Box marginTop={10}>
                <NextLink href="/auth/login">
                  <Button width="100%">Login</Button>
                </NextLink>
              </Box>
            </Box>
          </Flex>
        </PageContainer>
      </>
    );
  }

  return (
    <>
      <Head>
        <title>{title}</title>
      </Head>
      <PageContainer {...props}>{children}</PageContainer>
    </>
  );
};

export default Page;
