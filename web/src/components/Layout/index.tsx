import { Box } from "@chakra-ui/react";
import Head from "next/head";
import Router, { useRouter } from "next/router";
import React, { useEffect, useMemo } from "react";
import Container from "~/components/Container";
import Header from "~/components/Header";
import useUser from "~/lib/contexts/user_context";

interface PageProps {
  title?: string;
  loginRequired?: boolean;
  useHeader?: boolean;
  beforeContainer?: React.ReactNode;
}

const Page: React.FC<PageProps> = ({
  title = "Audiochan",
  loginRequired = false,
  useHeader = true,
  beforeContainer,
  children,
  ...props
}) => {
  const { isAuth } = useUser();
  const { asPath } = useRouter();

  const loginUrl = useMemo<string>(() => {
    return `/login?redirect=${decodeURIComponent(asPath)}`;
  }, [asPath]);

  useEffect(() => {
    if (!isAuth && loginRequired) {
      Router.push(loginUrl);
    }
  }, [isAuth, asPath, loginRequired]);

  return (
    <>
      <Head>
        <title>{title}</title>
      </Head>
      {useHeader && <Header title="Audiochan" />}
      {beforeContainer}
      <Container {...props}>
        <Box paddingX="5" paddingY="1.5rem">
          {children}
        </Box>
      </Container>
    </>
  );
};

export default Page;
