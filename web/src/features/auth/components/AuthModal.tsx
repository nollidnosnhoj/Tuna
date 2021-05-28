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
import React, { useEffect, useRef, useState } from "react";
import { useLoginModal } from "~/lib/stores";
import LoginForm from "./LoginForm";
import RegisterForm from "./RegisterForm";

export default function LoginModal() {
  const { modalState, open, onClose } = useLoginModal();
  const [tabIndex, setTabIndex] = useState(0);
  const authInputRef = useRef<HTMLInputElement | null>(null);

  useEffect(() => {
    switch (modalState) {
      case "login":
        setTabIndex(0);
        break;
      case "register":
        setTabIndex(1);
        break;
    }
  }, [modalState]);

  return (
    <Modal initialFocusRef={authInputRef} isOpen={open} onClose={onClose}>
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
            onChange={(index) => setTabIndex(index)}
          >
            <TabList>
              <Tab>Login</Tab>
              <Tab>Sign Up</Tab>
            </TabList>
            <TabPanels>
              <TabPanel>
                <LoginForm initialRef={authInputRef} onSuccess={onClose} />
              </TabPanel>
              <TabPanel>
                <RegisterForm initialRef={authInputRef} onSuccess={onClose} />
              </TabPanel>
            </TabPanels>
          </Tabs>
        </ModalBody>
      </ModalContent>
    </Modal>
  );
}
