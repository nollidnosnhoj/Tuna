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
import React, { PropsWithChildren } from "react";
import LoginForm from "./LoginForm";

interface LoginModalProps {
  buttonSize?: string;
}

export default function LoginModal({ buttonSize }: LoginModalProps) {
  const { isOpen, onOpen, onClose } = useDisclosure();

  return (
    <>
      <Button size={buttonSize} onClick={onOpen}>
        Login
      </Button>

      <Modal isOpen={isOpen} onClose={onClose}>
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>Login</ModalHeader>
          <ModalCloseButton />
          <ModalBody paddingBottom={5}>
            <LoginForm />
          </ModalBody>
        </ModalContent>
      </Modal>
    </>
  );
}
