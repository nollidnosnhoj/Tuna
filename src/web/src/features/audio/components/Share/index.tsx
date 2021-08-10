import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalCloseButton,
  ModalBody,
  Stack,
} from "@chakra-ui/react";
import React from "react";
import SETTINGS from "~/lib/config";
import AudioShareItem from "./Item";

interface AudioShareModalProps {
  isOpen: boolean;
  onClose: () => void;
  slug: string;
}

export default function AudioShareModal(props: AudioShareModalProps) {
  const { slug, isOpen, onClose } = props;

  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>Share</ModalHeader>
        <ModalCloseButton />
        <ModalBody marginY={4}>
          <Stack spacing={4} direction="column">
            <AudioShareItem
              label="URL"
              value={SETTINGS.FRONTEND_URL + `audios/${slug}`}
            />
          </Stack>
        </ModalBody>
      </ModalContent>
    </Modal>
  );
}
