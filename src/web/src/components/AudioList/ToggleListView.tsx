import { ButtonGroup, IconButton } from "@chakra-ui/button";
import React from "react";
import { HiViewGrid, HiViewList } from "react-icons/hi";
import { useAudioList } from ".";

export default function AudioListToggleView() {
  const { viewLayout, setViewLayout } = useAudioList();

  return (
    <ButtonGroup variant="outline" spacing="6">
      <IconButton
        aria-label="View List"
        icon={<HiViewList />}
        onClick={() => setViewLayout("list")}
        opacity={viewLayout === "list" ? 1 : 0.5}
      />
      <IconButton
        aria-label="View Grid"
        icon={<HiViewGrid />}
        onClick={() => setViewLayout("grid")}
        opacity={viewLayout === "grid" ? 1 : 0.5}
      />
    </ButtonGroup>
  );
}
