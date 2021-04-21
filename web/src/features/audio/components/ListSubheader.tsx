import { HStack, Button } from "@chakra-ui/react";
import NextLink from "next/link";
import React from "react";
import Container from "~/components/Container";
import { useAuth } from "~/contexts/AuthContext";

export default function AudioListSubHeader(props: { current: string }) {
  const { isLoggedIn } = useAuth();
  return (
    <Container paddingY={2}>
      <HStack spacing={4}>
        <NextLink href="/audios/latest">
          <Button
            size="lg"
            variant="link"
            colorScheme={props.current === "latest" ? "primary" : undefined}
          >
            Latest
          </Button>
        </NextLink>
        {isLoggedIn && (
          <NextLink href="/feed">
            <Button
              size="lg"
              variant="link"
              colorScheme={props.current === "feed" ? "primary" : undefined}
            >
              Your Feed
            </Button>
          </NextLink>
        )}
      </HStack>
    </Container>
  );
}
