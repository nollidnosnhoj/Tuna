import {
  Button,
  Modal,
  ModalBody,
  ModalCloseButton,
  ModalContent,
  ModalHeader,
  ModalOverlay,
  useDisclosure,
} from "@chakra-ui/react";
import React from "react";
import RegisterForm from "./RegisterForm";

interface RegisterModalProps {
  buttonSize?: string;
}

export default function RegisterModal({ buttonSize }: RegisterModalProps) {
  const { isOpen, onOpen, onClose } = useDisclosure();

  return (
    <>
      <Button colorScheme="primary" size={buttonSize} onClick={onOpen}>
        Register
      </Button>

      <Modal isOpen={isOpen} onClose={onClose}>
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>Register</ModalHeader>
          <ModalCloseButton />
          <ModalBody paddingBottom={5}>
            <RegisterForm />
          </ModalBody>
        </ModalContent>
      </Modal>
    </>
  );
}
