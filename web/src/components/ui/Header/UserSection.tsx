import {
  Box,
  Menu,
  MenuButton,
  IconButton,
  MenuList,
  MenuGroup,
  MenuItem,
  MenuDivider,
  Button,
} from "@chakra-ui/react";
import router from "next/router";
import React from "react";
import { FaUserAlt } from "react-icons/fa";
import NextLink from "next/link";
import { useUser } from "~/contexts/UserContext";
import { useAuth } from "~/contexts/AuthContext";
import { usePage } from "~/components/Page";

const UserSection: React.FC = () => {
  const { user } = useUser();
  const { isLoggedIn } = useAuth();
  const { openLogin, openRegister } = usePage();

  if (isLoggedIn) {
    return (
      <Box zIndex={4}>
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
              <NextLink href="/setting">
                <MenuItem>Settings</MenuItem>
              </NextLink>
            </MenuGroup>
            <MenuDivider />
            <MenuItem onClick={() => router.push("/auth/logout")}>
              Logout
            </MenuItem>
          </MenuList>
        </Menu>
      </Box>
    );
  }

  return (
    <>
      <Button
        size="md"
        colorScheme="gray"
        variant="ghost"
        textTransform="uppercase"
        onClick={openLogin}
      >
        Login
      </Button>
      <Button
        size="md"
        colorScheme="primary"
        textTransform="uppercase"
        onClick={openRegister}
      >
        Register
      </Button>
    </>
  );
};

export default UserSection;
