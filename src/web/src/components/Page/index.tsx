import React from "react";
import dynamic from "next/dynamic";
import Head from "next/head";
import NextLink from "next/link";
import {
  Box,
  BoxProps,
  Button,
  Drawer,
  DrawerContent,
  DrawerOverlay,
  Flex,
  Heading,
  useDisclosure,
} from "@chakra-ui/react";
import Header from "./Header";
import { useUser } from "~/features/user/hooks";
import Sidebar from "./Sidebar";

const AudioPlayer = dynamic(
  () => import("~/features/audio/components/Player"),
  {
    ssr: false,
  }
);

const PageContainer: React.FC<BoxProps> = ({ children, ...props }) => {
  const { isOpen, onOpen, onClose } = useDisclosure();

  return (
    <Box as="section" minHeight="100vh">
      <Sidebar display={{ base: "none", md: "unset" }} />
      <Drawer isOpen={isOpen} onClose={onClose} placement="left">
        <DrawerOverlay />
        <DrawerContent>
          <Sidebar width="full" borderRight="none" />
        </DrawerContent>
      </Drawer>
      <Box marginLeft={{ base: 0, md: 60 }}>
        <Header onOpenMenu={onOpen} />
        <Box {...props} p={4} paddingBottom="120px" as="main">
          {children}
        </Box>
      </Box>
      <AudioPlayer />
    </Box>
  );
};

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
  const { isLoggedIn } = useUser();

  if (requiresAuth && !isLoggedIn) {
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
