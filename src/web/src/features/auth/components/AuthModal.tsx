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
  useToast,
} from "@chakra-ui/react";
import React, { useEffect, useRef, useState } from "react";
import request from "~/lib/http";
import { useLoginModal } from "~/lib/stores";
import { errorToast } from "~/utils";
import { useLogin } from "../api/hooks";
import LoginForm, { LoginFormValues } from "./Forms/Login";
import RegisterForm, { RegisterFormInputs } from "./Forms/Register";

export default function LoginModal() {
  const toast = useToast();
  const { modalState, open, onClose } = useLoginModal();
  const [tabIndex, setTabIndex] = useState(0);
  const authInputRef = useRef<HTMLInputElement | null>(null);
  const { mutate: login } = useLogin();

  const handleLogin = async (values: LoginFormValues) => {
    login(values, {
      onSuccess() {
        toast({
          status: "success",
          description: "You have logged in successfully. ",
        });
        onClose();
      },
    });
  };

  const handleRegister = async (values: RegisterFormInputs) => {
    try {
      await request({
        method: "post",
        url: "auth/register",
        data: {
          username: values.username,
          password: values.password,
          email: values.email,
        },
      });
      toast({
        title: "Thank you for registering.",
        description: "You can now login to your account.",
        status: "success",
      });
      onClose();
    } catch (err) {
      errorToast(err);
    }
  };

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
                <LoginForm initialRef={authInputRef} onSubmit={handleLogin} />
              </TabPanel>
              <TabPanel>
                <RegisterForm
                  initialRef={authInputRef}
                  onSubmit={handleRegister}
                />
              </TabPanel>
            </TabPanels>
          </Tabs>
        </ModalBody>
      </ModalContent>
    </Modal>
  );
}
