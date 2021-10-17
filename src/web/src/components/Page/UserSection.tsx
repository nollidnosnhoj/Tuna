import {
  Button,
  Menu,
  MenuButton,
  MenuDivider,
  MenuGroup,
  MenuItem,
  MenuList,
  Portal,
} from "@chakra-ui/react";
import { ChevronDownIcon } from "@chakra-ui/icons";
import router from "next/router";
import React from "react";
import NextLink from "next/link";
import { useUser } from "~/components/providers/UserProvider";

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
            <NextLink href="/me/settings">
              <MenuItem>Settings</MenuItem>
            </NextLink>
          </MenuGroup>
          <MenuDivider />
          <MenuItem onClick={() => router.push("/logout")}>Logout</MenuItem>
        </MenuList>
      </Portal>
    </Menu>
  );
};

export default UserSection;
