import React from "react";
import { FaCloudUploadAlt, FaUserAlt } from "react-icons/fa";
import {
  Box,
  Button,
  IconButton,
  Menu,
  MenuButton,
  MenuDivider,
  MenuGroup,
  MenuItem,
  MenuList,
  Stack,
  Text,
} from "@chakra-ui/react";
import NextLink from "next/link";
import useUser from "~/lib/contexts/user_context";
import AuthButton from "../Auth/AuthButton";
import ChangeThemeModeButton from "./ChangeThemeModeButton";
import VolumeSliderHeader from "./VolumeSliderHeader";

export default function UserHeader() {
  const { user, isLoading, isAuth, logout } = useUser();

  if (isLoading) {
    return <Text>Loading...</Text>;
  }

  return (
    <>
      <Stack direction="row" spacing={4}>
        <ChangeThemeModeButton />
        <VolumeSliderHeader />
        {!isAuth ? (
          <>
            <AuthButton authType="login" />
            <AuthButton authType="register" />
          </>
        ) : (
          <>
            <NextLink href="/upload">
              <Button leftIcon={<FaCloudUploadAlt />}>Upload</Button>
            </NextLink>
            <Box>
              <Menu>
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
                    <NextLink href={`/users/${user.username}`}>
                      <MenuItem>Profile</MenuItem>
                    </NextLink>
                    <NextLink href="/setting">
                      <MenuItem>Settings</MenuItem>
                    </NextLink>
                  </MenuGroup>
                  <MenuDivider />
                  <MenuItem onClick={() => logout()}>Logout</MenuItem>
                </MenuList>
              </Menu>
            </Box>
          </>
        )}
      </Stack>
    </>
  );
}
