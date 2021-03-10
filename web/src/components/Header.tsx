import { CloseIcon, HamburgerIcon, MoonIcon, SunIcon } from "@chakra-ui/icons";
import {
  Box,
  Button,
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
import React from "react";
import { FaCloudUploadAlt, FaUserAlt } from "react-icons/fa";
import { MdLibraryMusic } from "react-icons/md";
import NextLink from "next/link";
import Link from "./Link";
import useUser from "~/contexts/userContext";
import Container from "./Container";

interface HeaderNavLinkProps {
  href: string;
  color?: string;
  icon?: React.ReactElement;
}

const HeaderNavLink: React.FC<HeaderNavLinkProps> = ({
  href,
  color,
  icon,
  children,
}) => (
  <NextLink href={href}>
    <Button variant="ghost" colorScheme={color} size="md" leftIcon={icon}>
      {children}
    </Button>
  </NextLink>
);

interface HeaderProps {}

const Header: React.FC<HeaderProps> = (props) => {
  const { user, logout } = useUser();
  const { toggleColorMode } = useColorMode();
  const ColorModeIcon = useColorModeValue(MoonIcon, SunIcon);
  const { isOpen, onOpen, onClose } = useDisclosure();

  const HeaderNavLinks = (
    <HStack spacing={2}>
      <HeaderNavLink href="/audios" icon={<MdLibraryMusic />}>
        Browse
      </HeaderNavLink>
      {user && (
        <HeaderNavLink
          color="primary"
          href="/upload"
          icon={<FaCloudUploadAlt />}
        >
          Upload
        </HeaderNavLink>
      )}
    </HStack>
  );

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
      <Box as="header" px={4} borderColor="primary.500" borderBottomWidth={4}>
        <Container py="3">
          <Flex h={16} alignItems="center" justifyContent="space-between">
            <IconButton
              variant="ghost"
              size="lg"
              icon={isOpen ? <CloseIcon /> : <HamburgerIcon />}
              aria-label="Open menu"
              display={{ md: !isOpen ? "none" : "inherit" }}
              onClick={isOpen ? onClose : onOpen}
            />
            <HStack as="nav" spacing={8} display={{ base: "none", md: "flex" }}>
              <Heading display={{ base: "none", md: "flex" }}>
                <Link
                  href="/"
                  _hover={{
                    textDecoration: "none",
                  }}
                >
                  Audiochan
                </Link>
              </Heading>
              {HeaderNavLinks}
            </HStack>
            <Flex alignItems="center">
              <HStack spacing={4}>
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
        </Container>
        {isOpen && (
          <Box pb={4}>
            <Stack as="nav" spacing={4}>
              {HeaderNavLinks}
            </Stack>
          </Box>
        )}
      </Box>
    </React.Fragment>
  );
};

export default Header;
