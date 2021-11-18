import React from "react";
import Head from "next/head";
import NextLink from "next/link";
import { Box, Button, Flex, Heading } from "@chakra-ui/react";
import { useRouter } from "next/router";
import { useUser } from "~/components/providers/UserProvider";
import { PageLayout } from "~/components/Page/Layout";

interface PageProps {
  title?: string;
  requiresAuth?: boolean;
  requiresArtist?: boolean;
}

const UnauthorizedPage = () => {
  const router = useRouter();
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
};

const ForbiddenPage = () => {
  return (
    <>
      <Head>
        <title>Restricted Access</title>
      </Head>
      <PageLayout>
        <Flex justify="center" align="center" height="50vh">
          <Box>
            <Heading as="h2">You are restricted from accessing.</Heading>
          </Box>
        </Flex>
      </PageLayout>
    </>
  );
};

const Page: React.FC<PageProps> = ({
  title = "Audiochan",
  requiresAuth = false,
  requiresArtist = false,
  children,
}) => {
  const { isLoggedIn, user } = useUser();

  if (requiresAuth && !isLoggedIn) {
    return <UnauthorizedPage />;
  }

  if (user && requiresArtist && !user.isArtist) {
    return <ForbiddenPage />;
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
