import {
  Box,
  BoxProps,
  CloseButton,
  Drawer,
  DrawerContent,
  Flex,
  FlexProps,
  Text,
} from "@chakra-ui/react";
import React, { ReactText } from "react";
import { useColorModeValue } from "@chakra-ui/color-mode";
import { IconType } from "react-icons";
import ChakraLink from "~/components/ui/Link";
import { Icon } from "@chakra-ui/icon";
import { MdHome } from "react-icons/md";
import { useDisclosure } from "@chakra-ui/hooks";
import { Header } from "~/components/Page/Header";

interface INavItemProps extends FlexProps {
  icon: IconType;
  children: ReactText;
  href: string;
}

function NavItem({ href, icon, children, ...rest }: INavItemProps) {
  return (
    <ChakraLink href={href} style={{ textDecoration: "none" }}>
      <Flex
        align="center"
        px="4"
        py="2"
        borderRadius="lg"
        role="group"
        cursor="pointer"
        _hover={{
          bg: "primary.400",
          color: "white",
        }}
        {...rest}
      >
        {icon && (
          <Icon
            mr="4"
            fontSize="16"
            _groupHover={{
              color: "white",
            }}
            as={icon}
          />
        )}
        {children}
      </Flex>
    </ChakraLink>
  );
}

interface ISidebarProps extends BoxProps {
  onClose: () => void;
}

const SidebarContent = ({ onClose, ...rest }: ISidebarProps) => {
  return (
    <Box
      bg={useColorModeValue("white", "gray.900")}
      borderRight="1px"
      borderRightColor={useColorModeValue("gray.200", "gray.700")}
      w={{ base: "full", md: 60 }}
      pos="fixed"
      h="full"
      {...rest}
    >
      <Flex as={"header"} h="20" alignItems="center" mx="8" justifyContent="space-between">
        <Text fontSize="2xl" fontWeight="bold">
          Audiochan
        </Text>
        <CloseButton display={{ base: "flex", md: "none" }} onClick={onClose} />
      </Flex>
      <Box as={"nav"} px={"4"}>
        <NavItem icon={MdHome} href="/">
          Home
        </NavItem>
      </Box>
    </Box>
  );
};

const PageLayout: React.FC = ({ children }) => {
  const { isOpen, onOpen, onClose } = useDisclosure();

  return (
    <Box minH="100vh" bg={useColorModeValue("white", "gray.900")}>
      <SidebarContent
        onClose={() => onClose}
        display={{ base: "none", md: "block" }}
      />
      <Drawer
        /* eslint-disable-next-line jsx-a11y/no-autofocus */
        autoFocus={false}
        isOpen={isOpen}
        placement="left"
        onClose={onClose}
        returnFocusOnClose={false}
        onOverlayClick={onClose}
        size="full"
      >
        <DrawerContent>
          <SidebarContent onClose={onClose} />
        </DrawerContent>
      </Drawer>
      <Header onOpen={onOpen} />
      <Box ml={{ base: 0, md: 60 }} p="4">
        {children}
      </Box>
    </Box>
  );
};

export { PageLayout };
