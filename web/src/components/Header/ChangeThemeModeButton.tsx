import React from "react";
import { Box, IconButton, useColorMode } from "@chakra-ui/react";
import { MoonIcon, SunIcon } from "@chakra-ui/icons";

export default function ChangeThemeModeButton() {
  const { colorMode, toggleColorMode } = useColorMode();

  return (
    <Box>
      {colorMode === "light" ? (
        <IconButton
          aria-label="Dark mode"
          icon={<MoonIcon />}
          variant="ghost"
          onClick={toggleColorMode}
        />
      ) : (
        <IconButton
          aria-label="Light mode"
          icon={<SunIcon />}
          variant="ghost"
          onClick={toggleColorMode}
        />
      )}
    </Box>
  );
}
