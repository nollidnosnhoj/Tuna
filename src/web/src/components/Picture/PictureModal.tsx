import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalCloseButton,
  ModalBody,
  Image,
  Flex,
} from "@chakra-ui/react";
import React from "react";
import PictureContainer from "./PictureContainer";

interface PictureModalProps {
  src: string;
  title: string;
  isOpen: boolean;
  onClose: () => void;
}

export default function PictureModal(props: PictureModalProps) {
  const { title, isOpen, onClose, src } = props;

  return (
    <Modal isOpen={isOpen} onClose={onClose} size="xl">
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>{title}</ModalHeader>
        <ModalCloseButton />
        <ModalBody marginY={4}>
          <Flex justify="center">
            <PictureContainer width="500px" height="500px">
              <Image src={src} maxW="500px" />
            </PictureContainer>
          </Flex>
        </ModalBody>
      </ModalContent>
    </Modal>
  );
}
