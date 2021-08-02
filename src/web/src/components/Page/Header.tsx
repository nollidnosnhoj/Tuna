import {
  Flex,
  IconButton,
  Stack,
  useColorMode,
  useColorModeValue,
} from "@chakra-ui/react";
import { HamburgerIcon, MoonIcon, SunIcon } from "@chakra-ui/icons";
import SearchBar from "./SearchBar";
import UserSection from "./UserSection";

interface HeaderProps {
  onOpenMenu?: () => void;
}

export default function Header({ onOpenMenu }: HeaderProps) {
  const { toggleColorMode } = useColorMode();
  const ColorModeIcon = useColorModeValue(MoonIcon, SunIcon);
  return (
    <Flex
      as="header"
      align="center"
      justify="space-between"
      width="full"
      paddingX={4}
      height={20}
    >
      <IconButton
        aria-label="Navigation Menu"
        display={{ base: "inline-flex", md: "none" }}
        icon={<HamburgerIcon />}
        onClick={onOpenMenu}
      />
      <SearchBar width="96" display={{ base: "none", md: "flex" }} />
      <Stack direction="row">
        <IconButton
          aria-label="Change color mode"
          icon={<ColorModeIcon />}
          size="md"
          variant="ghost"
          onClick={toggleColorMode}
        />
        <UserSection />
      </Stack>
    </Flex>
  );
}
