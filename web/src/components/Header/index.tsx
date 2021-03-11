import { CloseIcon, HamburgerIcon, MoonIcon, SunIcon } from "@chakra-ui/icons";
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
  Icon,
  IconButton,
  Menu,
  MenuButton,
  MenuDivider,
  MenuGroup,
  MenuItem,
  MenuList,
  Stack,
  useColorMode,
  useColorModeValue,
  useDisclosure,
} from "@chakra-ui/react";
import React, { useRef } from "react";
import { FaCloudUploadAlt, FaUserAlt } from "react-icons/fa";
import { MdLibraryMusic } from "react-icons/md";
import NextLink from "next/link";
import Link from "../Link";
import useUser from "~/hooks/useUser";
import Container from "../Container";
import HeaderMenuLink from "./MenuLink";

interface HeaderProps {}

const Header: React.FC<HeaderProps> = (props) => {
  const { user, logout } = useUser();
  const { toggleColorMode } = useColorMode();
  const ColorModeIcon = useColorModeValue(MoonIcon, SunIcon);
  const menuButtonRef = useRef<any>();
  const { isOpen, onOpen, onClose } = useDisclosure();

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
        borderColor="primary.500"
        borderBottomWidth={4}
      >
        <IconButton
          variant="ghost"
          size="lg"
          icon={isOpen ? <CloseIcon /> : <HamburgerIcon />}
          aria-label="Open menu"
          // display={{ md: !isOpen ? "none" : "inherit" }}
          onClick={isOpen ? onClose : onOpen}
          position="relative"
          marginRight={14}
        />
        <Flex
          width="100%"
          my={3}
          h={16}
          alignItems="center"
          justifyContent="space-between"
        >
          <HStack as="nav" spacing={8}>
            <Heading size="lg" display={{ base: "none", md: "flex" }}>
              <Link
                href="/"
                _hover={{
                  textDecoration: "none",
                }}
              >
                Audiochan
              </Link>
            </Heading>
          </HStack>
          <HStack spacing={4} marginRight={{ base: 2, md: 8 }}>
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
            <DrawerCloseButton />
            <DrawerHeader borderColor="primary.500" borderBottomWidth={4}>
              Menu
            </DrawerHeader>
            <DrawerBody p={0}>
              <HeaderMenuLink
                label="Browse"
                href="/audios"
                icon={<MdLibraryMusic />}
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
