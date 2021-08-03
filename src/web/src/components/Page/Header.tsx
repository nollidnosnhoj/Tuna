import { Flex, IconButton, Button, Stack } from "@chakra-ui/react";
import { HamburgerIcon } from "@chakra-ui/icons";
import SearchBar from "./SearchBar";
import UserSection from "./UserSection";
import ThemeModeButton from "./ThemeModeButton";
import { useUser } from "~/features/user/hooks";
import { useLoginModal } from "~/lib/stores";
import React from "react";

interface HeaderProps {
  onOpenMenu?: () => void;
}

export default function Header({ onOpenMenu }: HeaderProps) {
  const { isLoggedIn } = useUser();
  const openAuthModal = useLoginModal((state) => state.onOpen);

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
            <Button
              size="md"
              colorScheme="gray"
              variant="ghost"
              textTransform="uppercase"
              onClick={() => openAuthModal("login")}
            >
              Login
            </Button>
            <Button
              size="md"
              colorScheme="primary"
              textTransform="uppercase"
              onClick={() => openAuthModal("register")}
              display={{ base: "none", md: "flex" }}
            >
              Register
            </Button>
          </>
        )}
      </Stack>
    </Flex>
  );
}
