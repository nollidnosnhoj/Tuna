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
import { AudioId } from "../../types";
import AudioShareItem from "./AudioShareItem";

interface AudioShareModalProps {
  isOpen: boolean;
  onClose: () => void;
  audioId: AudioId;
}

export default function AudioShareModal(props: AudioShareModalProps) {
  const { audioId, isOpen, onClose } = props;

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
              value={SETTINGS.FRONTEND_URL + `audios/${audioId}`}
            />
          </Stack>
        </ModalBody>
      </ModalContent>
    </Modal>
  );
}
