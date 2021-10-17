import {
  Input,
  InputGroup,
  InputLeftAddon,
  Modal,
  ModalBody,
  ModalCloseButton,
  ModalContent,
  ModalHeader,
  ModalOverlay,
  Stack,
  useClipboard,
} from "@chakra-ui/react";
import React from "react";
import SETTINGS from "~/lib/config";
import { toast } from "~/utils";

interface AudioShareModalProps {
  isOpen: boolean;
  onClose: () => void;
  slug: string;
}

interface AudioShareItemProps {
  label: string;
  value: string;
}

function AudioShareItem(props: AudioShareItemProps) {
  const { onCopy } = useClipboard(props.value);

  return (
    <InputGroup>
      <InputLeftAddon>{props.label}</InputLeftAddon>
      <Input
        isReadOnly
        value={props.value}
        onClick={(event) => {
          event.currentTarget.select();
          onCopy();
          toast("info", {
            description: "Link copied.",
            duration: 3000,
            isClosable: false,
          });
        }}
      />
    </InputGroup>
  );
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
