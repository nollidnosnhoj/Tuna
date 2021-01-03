import { SearchIcon } from "@chakra-ui/icons";
import {
  Box,
  Flex,
  Heading,
  IconButton,
  Input,
  InputGroup,
  InputRightElement,
} from "@chakra-ui/react";
import NextLink from "next/link";
import React, { useState } from "react";
import Container from "~/components/Container";
import Link from "~/components/Link";
import HeaderUser from "~/components/HeaderUser";

interface HeaderProps {
  title: string;
  logo?: string;
}

const Header: React.FC<HeaderProps> = ({ title, logo, children, ...props }) => {
  // const [searchTerm, setSearchTerm] = useState("");

  // const onKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
  //   if (e.key === "Enter") handleSearch();
  // };

  // const handleSearch = () => {
  //   if (searchTerm === "") return;

  //   console.log(searchTerm);
  // };

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
          {/* <Box marginLeft="5">
            <InputGroup>
              <Input
                paddingLeft="2"
                placeholder="Search"
                value={searchTerm}
                onKeyDown={onKeyDown}
                onChange={(e) => setSearchTerm(e.target.value)}
                variant="flushed"
              />
              <InputRightElement>
                <IconButton
                  size="sm"
                  aria-label="Search"
                  icon={<SearchIcon />}
                  variant="ghost"
                  onClick={handleSearch}
                />
              </InputRightElement>
            </InputGroup>
          </Box> */}
          <div style={{ flexGrow: 1 }}></div>
          <HeaderUser />
        </Flex>
      </Container>
    </Box>
  );
};

export default Header;
