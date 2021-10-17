import { IconButton } from "@chakra-ui/button";
import { useDisclosure } from "@chakra-ui/hooks";
import React from "react";
import { FaShare } from "react-icons/fa";
import AudioShareModal from "../modals/AudioShareModal";
import { AudioView } from "~/lib/types";

interface AudioShareButtonProps {
  audio: AudioView;
}

export default function AudioShareButton({ audio }: AudioShareButtonProps) {
  const {
    isOpen: isShareOpen,
    onOpen: onShareOpen,
    onClose: onShareClose,
  } = useDisclosure();

  return (
    <>
      <IconButton
        aria-label="Share Audio"
        icon={<FaShare />}
        isRound
        variant="ghost"
        onClick={onShareOpen}
      />
      <AudioShareModal
        slug={audio.slug}
        isOpen={isShareOpen}
        onClose={onShareClose}
      />
    </>
  );
}
