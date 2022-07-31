import { useColorModeValue } from "@chakra-ui/color-mode";
import { FiChevronDown, FiMenu } from "react-icons/fi";
import { MoonIcon, SunIcon } from "@chakra-ui/icons";
import { MdCloudUpload } from "react-icons/md";
import React from "react";
import {
  Avatar,
  Flex,
  HStack,
  IconButton,
  Menu,
  MenuButton,
  VStack,
  Text,
  Box,
  MenuList,
  MenuItem,
  MenuDivider,
  FlexProps,
  Button,
  useColorMode,
  MenuGroup,
} from "@chakra-ui/react";
import NextLink from "next/link";
import { useRouter } from "next/router";
import { CurrentUser } from "~/lib/types";
import { useUser } from "~/components/providers/UserProvider";

interface IHeaderProps extends FlexProps {
  onOpen: () => void;
}

interface IAuthenticatedHeaderSectionProps {
  user: CurrentUser;
}

function AuthenticatedHeaderSection({
  user,
}: IAuthenticatedHeaderSectionProps) {
  return (
    <Flex alignItems={"center"}>
      <Menu>
        <MenuButton py={2} transition="all 0.3s" _focus={{ boxShadow: "none" }}>
          <HStack>
            <Avatar size={"sm"} name={user.userName} />
            <VStack
              display={{ base: "none", md: "flex" }}
              alignItems="flex-start"
              spacing="1px"
              ml="2"
            >
              <Text fontSize="sm">{user.userName}</Text>
            </VStack>
            <Box display={{ base: "none", md: "flex" }}>
              <FiChevronDown />
            </Box>
          </HStack>
        </MenuButton>
        <MenuList
          bg={useColorModeValue("white", "gray.900")}
          borderColor={useColorModeValue("gray.200", "gray.700")}
        >
          <MenuGroup>
            <NextLink href={`/artists/${user.userName}`}>
              <MenuItem>Profile</MenuItem>
            </NextLink>
            <NextLink href="/me/audios">
              <MenuItem>My Uploads</MenuItem>
            </NextLink>
          </MenuGroup>
          <MenuDivider />
          <MenuGroup>
            <NextLink href="/me">
              <MenuItem>Dashboard</MenuItem>
            </NextLink>
            <NextLink href="/me/settings">
              <MenuItem>Settings</MenuItem>
            </NextLink>
          </MenuGroup>
          <MenuDivider />
          <NextLink href="/logout">
            <MenuItem>Sign out</MenuItem>
          </NextLink>
        </MenuList>
      </Menu>
    </Flex>
  );
}

export function Header({ onOpen, ...rest }: IHeaderProps) {
  const router = useRouter();
  const { user } = useUser();
  const { toggleColorMode } = useColorMode();
  const ColorModeIcon = useColorModeValue(SunIcon, MoonIcon);
  return (
    <Flex
      as={"header"}
      ml={{ base: 0, md: 60 }}
      px={{ base: 4, md: 4 }}
      height="20"
      alignItems="center"
      bg={useColorModeValue("white", "gray.900")}
      borderBottomWidth="1px"
      borderBottomColor={useColorModeValue("gray.200", "gray.700")}
      justifyContent={{ base: "space-between", md: "flex-end" }}
      {...rest}
    >
      <IconButton
        display={{ base: "flex", md: "none" }}
        onClick={onOpen}
        variant="outline"
        aria-label="open menu"
        icon={<FiMenu />}
      />
      <HStack spacing={{ base: "4", md: "6" }}>
        {user && (
          <NextLink href={"/upload"}>
            <Button leftIcon={<MdCloudUpload />} colorScheme={"primary"}>
              Upload
            </Button>
          </NextLink>
        )}
        <IconButton
          aria-label={"Switch light/dark theme"}
          icon={<ColorModeIcon />}
          onClick={toggleColorMode}
          variant={"ghost"}
          isRound
        />
        {user ? (
          <AuthenticatedHeaderSection user={user} />
        ) : (
          <React.Fragment>
            <NextLink href={`/login?redirecturl=${router.asPath}`}>
              <Button
                as={"a"}
                fontSize={"sm"}
                fontWeight={400}
                variant={"link"}
                cursor={"pointer"}
              >
                Sign In
              </Button>
            </NextLink>
            <NextLink href="/signup/user">
              <Button
                display={{ base: "none", md: "inline-flex" }}
                fontSize={"sm"}
                fontWeight={600}
                color={"white"}
                bg={"primary.400"}
                _hover={{
                  bg: "pink.300",
                }}
              >
                Sign Up
              </Button>
            </NextLink>
          </React.Fragment>
        )}
      </HStack>
    </Flex>
  );
}
