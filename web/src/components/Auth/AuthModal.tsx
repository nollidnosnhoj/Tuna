import {
  Box,
  Flex,
  Modal,
  ModalBody,
  ModalCloseButton,
  ModalContent,
  ModalFooter,
  ModalHeader,
  ModalOverlay,
  Link as ChakraLink,
} from "@chakra-ui/react";
import React, { useState } from "react";
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
  const [authType, setAuthType] = useState<AuthCommonType>(type);

  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>
          {authType === "login" && "Login"}
          {authType === "register" && "Register"}
        </ModalHeader>
        <ModalCloseButton />
        <ModalBody paddingBottom={5}>
          {authType === "login" && <LoginForm />}
          {authType === "register" && <RegisterForm />}
        </ModalBody>
        <ModalFooter alignItems="center" justifyContent="center">
          <Box textAlign="center">
            {authType === "login" && (
              <ChakraLink onClick={() => setAuthType("register")}>
                Do not have an account? Create an account today.
              </ChakraLink>
            )}
            {authType === "register" && (
              <ChakraLink onClick={() => setAuthType("login")}>
                Already have an account? Login here.
              </ChakraLink>
            )}
          </Box>
        </ModalFooter>
      </ModalContent>
    </Modal>
  );
}
