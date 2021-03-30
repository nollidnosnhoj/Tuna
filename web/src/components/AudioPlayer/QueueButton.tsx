import { chakra, Icon, Tooltip } from "@chakra-ui/react";
import React from "react";
import { MdQueue } from "react-icons/md";

interface QueueButtonProps {
  isOpen: boolean;
  onToggle: () => void;
}

export default function QueueButton({ onToggle, isOpen }: QueueButtonProps) {
  return (
    <Tooltip label={isOpen ? "Close queue panel" : "Open queue panel"}>
      <span>
        <chakra.button onClick={onToggle} className="rhap_repeat-button">
          <Icon as={MdQueue} aria-label="Toggle Queue" />
        </chakra.button>
      </span>
    </Tooltip>
  );
}
