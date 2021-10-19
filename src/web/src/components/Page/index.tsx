import React from "react";
import dynamic from "next/dynamic";
import Head from "next/head";
import NextLink from "next/link";
import { Box, Button, Flex, Heading } from "@chakra-ui/react";
import { useRouter } from "next/router";
import { useUser } from "~/components/providers/UserProvider";
import { PageLayout } from "~/components/Page/Layout";

interface PageProps {
  title?: string;
  requiresAuth?: boolean;
}

const Page: React.FC<PageProps> = ({
  title = "Audiochan",
  requiresAuth = false,
  children,
}) => {
  const { isLoggedIn } = useUser();
  const router = useRouter();

  if (requiresAuth && !isLoggedIn) {
    return (
      <>
        <Head>
          <title>Unauthorized</title>
        </Head>
        <PageLayout>
          <Flex justify="center" align="center" height="50vh">
            <Box>
              <Heading as="h2">You are not authorized.</Heading>
              <Box marginTop={10}>
                <NextLink href={`/login?redirecturl=${router.asPath}`}>
                  <Button width="100%">Login</Button>
                </NextLink>
              </Box>
            </Box>
          </Flex>
        </PageLayout>
      </>
    );
  }

  return (
    <>
      <Head>
        <title>{title}</title>
      </Head>
      <PageLayout>{children}</PageLayout>
    </>
  );
};

export default Page;
