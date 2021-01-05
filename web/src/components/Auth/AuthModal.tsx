import {
  Modal,
  ModalBody,
  ModalCloseButton,
  ModalContent,
  ModalHeader,
  ModalOverlay,
} from "@chakra-ui/react";
import React from "react";
import RegisterForm from "./RegisterForm";
import { AuthCommonType } from "./AuthButton";
import LoginForm from "./LoginForm";

export interface LoginModalProps {
  type: AuthCommonType;
  isOpen: boolean;
  onClose?: () => void;
}

export default function AuthModal(props: LoginModalProps) {
  const { type, isOpen, onClose } = props;

  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>
          {type === "login" && "Login"}
          {type === "register" && "Register"}
        </ModalHeader>
        <ModalCloseButton />
        <ModalBody paddingBottom={5}>
          {type === "login" && <LoginForm />}
          {type === "register" && <RegisterForm />}
        </ModalBody>
      </ModalContent>
    </Modal>
  );
}
