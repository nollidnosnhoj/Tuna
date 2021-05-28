import { Heading, Link } from "@chakra-ui/react";
import React from "react";

export default function Logo() {
  return (
    <Heading size="lg" display={{ base: "none", md: "flex" }}>
      <Link
        href="/"
        _hover={{
          textDecoration: "none",
        }}
      >
        Audiochan
      </Link>
    </Heading>
  );
}
