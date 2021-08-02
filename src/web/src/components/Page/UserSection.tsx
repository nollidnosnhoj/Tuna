import {
  Menu,
  MenuButton,
  IconButton,
  MenuList,
  MenuGroup,
  MenuItem,
  MenuDivider,
  Button,
  Stack,
} from "@chakra-ui/react";
import router from "next/router";
import React from "react";
import { FaUserAlt } from "react-icons/fa";
import NextLink from "next/link";
import { useUser } from "~/features/user/hooks/useUser";
import { useLoginModal } from "~/lib/stores";

const UserSection: React.FC = () => {
  const { user, isLoggedIn } = useUser();
  const openAuthModal = useLoginModal((state) => state.onOpen);

  if (isLoggedIn) {
    return (
      <Stack direction="row" spacing={2} align="center" zIndex={4}>
        <Menu placement="bottom-end">
          <MenuButton
            as={IconButton}
            icon={<FaUserAlt />}
            variant="ghost"
            colorScheme="primary"
          >
            Profile
          </MenuButton>
          <MenuList>
            <MenuGroup title={user?.username}>
              <NextLink href={`/users/${user?.username}`}>
                <MenuItem>Profile</MenuItem>
              </NextLink>
              <NextLink href="/settings">
                <MenuItem>Settings</MenuItem>
              </NextLink>
            </MenuGroup>
            <MenuDivider />
            <MenuItem onClick={() => router.push("/auth/logout")}>
              Logout
            </MenuItem>
          </MenuList>
        </Menu>
      </Stack>
    );
  }

  return (
    <Stack direction="row" spacing={2} align="center">
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
    </Stack>
  );
};

export default UserSection;
