import { SearchIcon } from "@chakra-ui/icons";
import {
  Box,
  BoxProps,
  Collapse,
  Flex,
  Icon,
  useColorModeValue,
  useDisclosure,
} from "@chakra-ui/react";
import NextLink from "next/link";
import React from "react";
import { FaCloudUploadAlt } from "react-icons/fa";
import { MdKeyboardArrowRight } from "react-icons/md";
import { useUser } from "~/features/user/hooks";
import Logo from "./Logo";

const LinkOrNot: React.FC<{ href?: string }> = ({ href, children }) => {
  if (!href) return <>{children}</>;

  return <NextLink href={href}>{children}</NextLink>;
};

interface NavLinkProps extends BoxProps {
  icon?: any;
  link?: string;
  label: string;
  isCollapse?: boolean;
}

const NavLink: React.FC<NavLinkProps> = ({
  icon,
  link,
  label,
  isCollapse = false,
  children,
  ...props
}) => {
  const disclosure = useDisclosure();
  const itemColor = useColorModeValue("inherit", "gray.400");
  const itemHoverBackgroundColor = useColorModeValue("gray.100", "gray.900");
  const itemHoverColor = useColorModeValue("gray.900", "gray.200");

  return (
    <>
      <LinkOrNot href={link}>
        <Flex
          align="center"
          paddingX={4}
          paddingLeft={4}
          paddingY={2}
          marginY={1}
          marginX={2}
          cursor="pointer"
          role="group"
          fontWeight="semibold"
          onClick={isCollapse ? disclosure.onToggle : undefined}
          color={itemColor}
          _hover={{
            bg: itemHoverBackgroundColor,
            color: itemHoverColor,
          }}
          transition=".15s ease"
          borderRadius={8}
          {...props}
        >
          {icon && <Icon marginRight={2} boxSize={4} as={icon} />}
          {label}
          {isCollapse && <Icon as={MdKeyboardArrowRight} marginLeft="auto" />}
        </Flex>
      </LinkOrNot>
      {isCollapse && <Collapse in={disclosure.isOpen}>{children}</Collapse>}
    </>
  );
};

const Sidebar: React.FC<BoxProps> = (props) => {
  const { isLoggedIn } = useUser();
  return (
    <Box
      as="nav"
      position="fixed"
      top={0}
      left={0}
      zIndex="sticky"
      height="full"
      overflowX="hidden"
      overflowY="auto"
      width={60}
      borderRightWidth="1px"
      paddingBottom={10}
      {...props}
    >
      <Flex align="center" justify="center" paddingX={4} paddingY={5}>
        <Logo />
      </Flex>
      <Flex direction="column" as="nav" aria-label="Navigation Menu">
        <NavLink icon={SearchIcon} link="/search" label="Search" />
        {isLoggedIn && (
          <NavLink icon={FaCloudUploadAlt} link="/upload" label="Upload" />
        )}
      </Flex>
    </Box>
  );
};

export default Sidebar;
