import React, { useCallback, useEffect, useState } from "react";
import {
  IconButton,
  Input,
  InputGroup,
  InputRightElement,
} from "@chakra-ui/react";
import { useRouter } from "next/router";
import { stringifyUrl } from "query-string";
import { SearchIcon } from "@chakra-ui/icons";

export default function SearchBar() {
  const { query, push: routerPush } = useRouter();
  const [term, setTerm] = useState<string | string[] | undefined>("");

  useEffect(() => {
    setTerm(query["q"]);
  }, [query]);

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") {
      e.preventDefault();
      handleSearch();
    }
  };

  const handleSearch = useCallback(() => {
    if (!term) return;
    routerPush(
      stringifyUrl({
        url: "/search",
        query: { q: term },
      })
    );
  }, [term, query]);

  return (
    <InputGroup size="md" alignItems="center">
      <Input
        size="lg"
        variant="filled"
        placeholder="Search..."
        value={term}
        onChange={(e) => setTerm(e.target.value)}
        onKeyDown={handleKeyDown}
        _hover={{
          boxShadow: "md",
        }}
      />
      <InputRightElement width="4.5rem">
        <IconButton
          h="1.75rem"
          aria-label="Search"
          onClick={handleSearch}
          icon={<SearchIcon />}
          variant="unstyled"
        />
      </InputRightElement>
    </InputGroup>
  );
}
