import React from "react";
import { NextPage } from "next";
import { Flex, Heading, Text } from "@chakra-ui/react";
import Page from "~/components/Page";

const NotFoundPage: NextPage = () => {
  return (
    <Page title="Not Found">
      <Flex alignItems="center" justifyContent="center">
        <Heading as="h1">You reached a dead end.</Heading>
        <Text as="p">You must have taken the wrong turn.</Text>
      </Flex>
    </Page>
  );
};

export default NotFoundPage;
