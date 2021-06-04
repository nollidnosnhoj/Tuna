import { Heading } from "@chakra-ui/react";
import React from "react";
import ChakraLink from "./Link";

export default function Logo() {
  return (
    <Heading size="lg" display={{ base: "none", md: "flex" }}>
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
