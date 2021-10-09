import {
  Menu,
  MenuButton,
  MenuList,
  MenuGroup,
  MenuItem,
  MenuDivider,
  Portal,
  Button,
} from "@chakra-ui/react";
import { ChevronDownIcon } from "@chakra-ui/icons";
import router from "next/router";
import React from "react";
import NextLink from "next/link";
import { useUser } from "~/features/user/hooks/useUser";

const UserSection: React.FC = () => {
  const { user } = useUser();
  return (
    <Menu placement="bottom-end" isLazy>
      <MenuButton as={Button} rightIcon={<ChevronDownIcon />}>
        {user?.userName}
      </MenuButton>
      <Portal>
        <MenuList>
          <MenuGroup>
            <NextLink href={`/users/${user?.userName}`}>
              <MenuItem>Profile</MenuItem>
            </NextLink>
            <NextLink href="/you/settings">
              <MenuItem>Settings</MenuItem>
            </NextLink>
          </MenuGroup>
          <MenuDivider />
          <MenuItem onClick={() => router.push("/auth/logout")}>
            Logout
          </MenuItem>
        </MenuList>
      </Portal>
    </Menu>
  );
};

export default UserSection;
