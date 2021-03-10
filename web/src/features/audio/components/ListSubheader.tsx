import { HStack, Button } from "@chakra-ui/react";
import NextLink from "next/link";
import React from "react";
import Container from "~/components/Container";
import useUser from "~/contexts/userContext";

export default function AudioListSubHeader(props: { current: string }) {
  const { isLoggedIn } = useUser();
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
        <NextLink href="/audios/favorites">
          <Button
            size="lg"
            variant="link"
            colorScheme={props.current === "favorites" ? "primary" : undefined}
          >
            Favorites
          </Button>
        </NextLink>
        {isLoggedIn && (
          <NextLink href="/audios/feed">
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
