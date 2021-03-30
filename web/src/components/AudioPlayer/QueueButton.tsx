import { chakra, Icon, Tooltip } from "@chakra-ui/react";
import React from "react";
import { MdPlaylistPlay } from "react-icons/md";

interface QueueButtonProps {
  onToggle: () => void;
}

export default function QueueButton({ onToggle }: QueueButtonProps) {
  return (
    <chakra.button
      onClick={onToggle}
      className="rhap_repeat-button"
      alt="Audio Queue"
      title="Audio Queue"
    >
      <Icon as={MdPlaylistPlay} aria-label="Toggle Queue" />
    </chakra.button>
  );
}
