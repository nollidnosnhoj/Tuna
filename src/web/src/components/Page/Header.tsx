import { Flex, IconButton, Button, Stack } from "@chakra-ui/react";
import { HamburgerIcon } from "@chakra-ui/icons";
import SearchBar from "./SearchBar";
import UserSection from "./UserSection";
import ThemeModeButton from "./ThemeModeButton";
import { useUser } from "~/features/user/hooks";
import React from "react";
import NextLink from "next/link";
import { useRouter } from "next/router";

interface HeaderProps {
  onOpenMenu?: () => void;
}

export default function Header({ onOpenMenu }: HeaderProps) {
  const router = useRouter();
  const { isLoggedIn } = useUser();

  return (
    <Flex
      as="header"
      align="center"
      justify="space-between"
      width="full"
      paddingX={4}
      paddingY={5}
      height={20}
    >
      <IconButton
        aria-label="Navigation Menu"
        display={{ base: "inline-flex", md: "none" }}
        icon={<HamburgerIcon />}
        onClick={onOpenMenu}
      />
      <SearchBar width="96" display={{ base: "none", md: "flex" }} />
      <Stack direction="row" spacing={2}>
        <ThemeModeButton />
        {isLoggedIn ? (
          <UserSection />
        ) : (
          <>
            <NextLink href={`/login?redirecturl=${router.asPath}`}>
              <Button
                size="md"
                colorScheme="gray"
                variant="ghost"
                textTransform="uppercase"
              >
                Login
              </Button>
            </NextLink>
            <NextLink href="/register">
              <Button
                size="md"
                colorScheme="primary"
                textTransform="uppercase"
                display={{ base: "none", md: "flex" }}
              >
                Register
              </Button>
            </NextLink>
          </>
        )}
      </Stack>
    </Flex>
  );
}
