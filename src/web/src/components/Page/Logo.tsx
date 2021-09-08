import { Heading } from "@chakra-ui/react";
import React from "react";
import ChakraLink from "../UI/Link";

export default function Logo() {
  return (
    <Heading size="lg">
      <ChakraLink
        href="/"
        _hover={{
          textDecoration: "none",
        }}
      >
        Audiochan
      </ChakraLink>
    </Heading>
  );
}
