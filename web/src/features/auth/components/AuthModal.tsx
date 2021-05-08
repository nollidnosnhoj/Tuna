import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalCloseButton,
  ModalBody,
  Tabs,
  TabList,
  Tab,
  TabPanels,
  TabPanel,
} from "@chakra-ui/react";
import React, { useRef } from "react";
import LoginForm from "./LoginForm";
import RegisterForm from "./RegisterForm";

interface AuthModalProps {
  isOpen: boolean;
  onClose: () => void;
  tabIndex: number;
  onTabChange: (i: number) => void;
}

export default function AuthModal({
  isOpen,
  onClose,
  tabIndex,
  onTabChange,
}: AuthModalProps) {
  const authInputRef = useRef<HTMLInputElement | null>(null);

  return (
    <Modal initialFocusRef={authInputRef} isOpen={isOpen} onClose={onClose}>
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>
          {tabIndex === 0 && "Login"}
          {tabIndex === 1 && "Sign Up"}
        </ModalHeader>
        <ModalCloseButton />
        <ModalBody>
          <Tabs
            isLazy
            isFitted
            index={tabIndex}
            onChange={(index) => onTabChange(index)}
          >
            <TabList>
              <Tab>Login</Tab>
              <Tab>Sign Up</Tab>
            </TabList>
            <TabPanels>
              <TabPanel>
                <LoginForm initialRef={authInputRef} />
              </TabPanel>
              <TabPanel>
                <RegisterForm initialRef={authInputRef} />
              </TabPanel>
            </TabPanels>
          </Tabs>
        </ModalBody>
      </ModalContent>
    </Modal>
  );
}
