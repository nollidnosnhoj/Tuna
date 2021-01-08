import { Box, Flex, Heading } from "@chakra-ui/react";
import React from "react";
import Container from "~/components/Shared/Container";
import Link from "~/components/Shared/Link";
import UserHeader from "~/components/Header/UserHeader";

interface HeaderProps {
  title: string;
  logo?: string;
}

const Header: React.FC<HeaderProps> = ({ title, logo, children, ...props }) => {
  return (
    <Box
      as="header"
      paddingY="1.5rem"
      borderBottomWidth="3px"
      borderColor="primary.400"
      {...props}
    >
      <Container pb="0" pt="0">
        <Flex align="center" grow={1} wrap="wrap" paddingX="6">
          <Link href="/" _hover={{ textDecoration: "none" }}>
            <Heading as="h1" size="lg" letterSpacing="2px">
              {title}
            </Heading>
          </Link>
          <div style={{ flexGrow: 1 }}></div>
          <UserHeader />
        </Flex>
      </Container>
    </Box>
  );
};

export default Header;
