import {
  CloseIcon,
  HamburgerIcon,
  MoonIcon,
  SearchIcon,
  SunIcon,
} from "@chakra-ui/icons";
import {
  Box,
  Button,
  Drawer,
  DrawerBody,
  DrawerCloseButton,
  DrawerContent,
  DrawerHeader,
  DrawerOverlay,
  Flex,
  Heading,
  HStack,
  IconButton,
  Input,
  Menu,
  MenuButton,
  MenuDivider,
  MenuGroup,
  MenuItem,
  MenuList,
  useColorMode,
  useColorModeValue,
  useDisclosure,
} from "@chakra-ui/react";
import React, { useCallback, useRef, useState } from "react";
import { FaCloudUploadAlt, FaUserAlt } from "react-icons/fa";
import { MdLibraryMusic } from "react-icons/md";
import Router, { useRouter } from "next/router";
import NextLink from "next/link";
import queryString from "query-string";
import Link from "../Link";
import useUser from "~/hooks/useUser";
import HeaderMenuLink from "./MenuLink";

interface HeaderProps {
  removeSearchBar?: boolean;
}

const Header: React.FC<HeaderProps> = (props) => {
  const router = useRouter();
  const { user, logout } = useUser();
  const { toggleColorMode } = useColorMode();
  const ColorModeIcon = useColorModeValue(MoonIcon, SunIcon);
  const menuButtonRef = useRef<any>();
  const { isOpen, onOpen, onClose, onToggle } = useDisclosure();
  const [searchTerm, setSearchTerm] = useState("");

  const handleSearchChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => {
      setSearchTerm(e.target.value);
    },
    [setSearchTerm]
  );

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") {
      e.preventDefault();
      if (!searchTerm) return;
      const { query } = router;
      const qs = queryString.stringify({ ...query, q: searchTerm });
      Router.push("/search?" + qs);
    }
  };

  const UserMenu = user ? (
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
          <MenuGroup title={user.username}>
            <NextLink href={`/users/${user.username}`}>
              <MenuItem>Profile</MenuItem>
            </NextLink>
            <NextLink href="/setting">
              <MenuItem>Settings</MenuItem>
            </NextLink>
          </MenuGroup>
          <MenuDivider />
          <MenuItem onClick={async () => await logout()}>Logout</MenuItem>
        </MenuList>
      </Menu>
    </Box>
  ) : (
    <>
      <NextLink href="/login">
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
        <Button size="md" colorScheme="primary" textTransform="uppercase">
          Register
        </Button>
      </NextLink>
    </>
  );

  return (
    <React.Fragment>
      <Flex
        align="center"
        as="header"
        px={4}
        position="fixed"
        top={0}
        left={0}
        width="100%"
        boxShadow="md"
        zIndex={1450}
      >
        <Flex
          width="100%"
          my={3}
          h={16}
          alignItems="center"
          justifyContent="space-between"
        >
          <Flex align="center" width={{ md: "full" }}>
            <IconButton
              variant="ghost"
              size="lg"
              icon={<HamburgerIcon />}
              aria-label="Open menu"
              onClick={onToggle}
              position="relative"
              ref={menuButtonRef}
              isRound
            />
            <Box display={{ base: "none", md: "flex" }} marginLeft={14}>
              <Heading size="lg">
                <Link
                  href="/"
                  _hover={{
                    textDecoration: "none",
                  }}
                >
                  Audiochan
                </Link>
              </Heading>
            </Box>
          </Flex>
          <Flex marginX={8} justify="center" width="full">
            <Box width="100%">
              {!props.removeSearchBar && (
                <Input
                  size="lg"
                  variant="filled"
                  placeholder="Search..."
                  value={searchTerm}
                  onChange={handleSearchChange}
                  onKeyDown={handleKeyDown}
                  _hover={{
                    boxShadow: "md",
                  }}
                />
              )}
            </Box>
          </Flex>
          <HStack
            spacing={4}
            marginRight={{ base: 2, md: 8 }}
            width={{ md: "full" }}
            justifyContent="flex-end"
          >
            <IconButton
              aria-label="Change color mode"
              icon={<ColorModeIcon />}
              size="md"
              variant="ghost"
              onClick={toggleColorMode}
            />
            {UserMenu}
          </HStack>
        </Flex>
      </Flex>
      <Drawer
        placement="left"
        onClose={onClose}
        isOpen={isOpen}
        initialFocusRef={menuButtonRef}
      >
        <DrawerOverlay>
          <DrawerContent>
            <DrawerBody p={0} marginTop="100px">
              <HeaderMenuLink
                label="Browse"
                href="/audios"
                icon={<MdLibraryMusic />}
              />
              <HeaderMenuLink
                label="Search"
                href="/search"
                icon={<SearchIcon />}
              />
              <HeaderMenuLink
                label="Upload"
                href="/upload"
                icon={<FaCloudUploadAlt />}
                hidden={!user}
                variant="solid"
                colorScheme="primary"
              />
            </DrawerBody>
          </DrawerContent>
        </DrawerOverlay>
      </Drawer>
    </React.Fragment>
  );
};

export default Header;
