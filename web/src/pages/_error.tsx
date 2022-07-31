import React from "react";
import { NextPage } from "next";
import { Flex, Heading, Text } from "@chakra-ui/react";
import Page from "~/components/Page";

function getErrorMessage(statusCode?: number) {
  switch (statusCode) {
    case 401:
      return "You are not authorized to access this page.";
    case 403:
      return "You are forbidden access to this page.";
    case 404:
      return "The requested page was not found.";
    default:
      return "An error has occurred. Please contact the administrators immediately.";
  }
}

const ErrorPage: NextPage<{ statusCode?: number }> = ({ statusCode }) => {
  return (
    <Page title="Uh oh">
      <Flex alignItems="center" justifyContent="center">
        <Heading as="h1">{statusCode || "Error"}</Heading>
        <Text as="p">{getErrorMessage(statusCode)}</Text>
      </Flex>
    </Page>
  );
};

ErrorPage.getInitialProps = ({ res, err }) => {
  const statusCode = res ? res.statusCode : err ? err.statusCode : 404;
  return { statusCode };
};

export default ErrorPage;
