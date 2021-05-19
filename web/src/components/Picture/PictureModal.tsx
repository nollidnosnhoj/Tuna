import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalCloseButton,
  ModalBody,
  Image,
} from "@chakra-ui/react";
import React from "react";
import PictureContainer from "./PictureContainer";

interface PictureModalProps {
  src: string;
  isOpen: boolean;
  onClose: () => void;
}

export default function PictureModal(props: PictureModalProps) {
  const { isOpen, onClose, src } = props;

  return (
    <Modal isOpen={isOpen} onClose={onClose} size="xl">
      <ModalOverlay />
      <ModalContent>
        <ModalHeader></ModalHeader>
        <ModalCloseButton />
        <ModalBody marginTop={4}>
          <PictureContainer>
            <Image src={src} maxW="500px" />
          </PictureContainer>
        </ModalBody>
      </ModalContent>
    </Modal>
  );
}
