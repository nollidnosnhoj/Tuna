import React, { useEffect } from "react";
import { Box, Button, Flex, Heading, Text } from "@chakra-ui/react";
import Router from "next/router";
import { FaRandom } from "react-icons/fa";
import useUser from "~/lib/contexts/user_context";
import request from "~/lib/request";
import { isAxiosError } from "~/utils/axios";
import Page from "~/components/Layout";

const Index = () => {
  const { isAuth } = useUser();

  useEffect(() => {
    Router.prefetch("/feed");
    if (isAuth) Router.push("/feed");
  }, [isAuth]);

  const goToRandomAudio = async () => {
    try {
      const { data } = await request<string>("audios/random-id");
      Router.push(`/audios/${data}`);
    } catch (err) {
      if (!isAxiosError(err)) {
        console.log(err);
      }
    }
  };

  return (
    <Page>
      <Flex justify="center" align="center" height="75vh">
        <Box textAlign="center">
          <Heading size="4xl" marginBottom={4}>
            Audiochan
          </Heading>
          <Text fontSize="4xl">Upload music and share!</Text>
          <Flex justify="center" marginTop={4}>
            <Button
              leftIcon={<FaRandom />}
              colorScheme="primary"
              variant="solid"
              onClick={() => goToRandomAudio()}
            >
              I'm feeling lucky
            </Button>
          </Flex>
        </Box>
      </Flex>
    </Page>
  );
};

export default Index;
