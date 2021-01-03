import {
  Box,
  Button,
  Flex,
  Heading,
  IconButton,
  Stack,
  Text,
} from "@chakra-ui/react";
import Router, { useRouter } from "next/router";
import Head from "next/head";
import NextLink from "next/link";
import React from "react";
import request from "~/lib/request";
import { errorToast } from "~/utils/toast";
import { FaRandom } from "react-icons/fa";

const Index = () => {
  const router = useRouter();

  const goToRandomAudio = async () => {
    try {
      const { data } = await request<string>("audios/random-id");
      Router.push(`/audios/${data}`);
    } catch (err) {
      errorToast({ message: "Unknown error" });
    }
  };

  return (
    <React.Fragment>
      <Head>
        <title>Audiochan</title>
      </Head>
      <Flex justify="center" align="center" height="75vh">
        <Box textAlign="center">
          <Heading size="4xl" marginBottom={4}>
            Audiochan
          </Heading>
          <Text fontSize="4xl">Upload music and share!</Text>
          <Flex justify="center" marginTop={4}>
            <Stack direction="row">
              <NextLink href="/login">
                <Button size="lg">Login</Button>
              </NextLink>
              <NextLink href="/register">
                <Button size="lg" colorScheme="primary">
                  Register
                </Button>
              </NextLink>
              <IconButton
                size="lg"
                aria-label="Random audio"
                icon={<FaRandom />}
                onClick={() => goToRandomAudio()}
              />
            </Stack>
          </Flex>
        </Box>
      </Flex>
    </React.Fragment>
  );
};

export default Index;
