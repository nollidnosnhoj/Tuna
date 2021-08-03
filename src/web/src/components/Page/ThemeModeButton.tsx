import { MoonIcon, SunIcon } from "@chakra-ui/icons";
import { useColorMode, useColorModeValue, IconButton } from "@chakra-ui/react";
import React from "react";

export default function ThemeModeButton() {
  const { toggleColorMode } = useColorMode();
  const ColorModeIcon = useColorModeValue(MoonIcon, SunIcon);

  return (
    <IconButton
      aria-label="Change color mode"
      icon={<ColorModeIcon />}
      size="md"
      variant="ghost"
      onClick={toggleColorMode}
    />
  );
}
