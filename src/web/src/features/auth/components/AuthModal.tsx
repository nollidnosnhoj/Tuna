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
  Alert,
  AlertIcon,
  CloseButton,
  AlertDescription,
} from "@chakra-ui/react";
import React, { useEffect, useRef, useState } from "react";
import request from "~/lib/http";
import { useLoginModal } from "~/lib/stores";
import { getErrorMessage } from "~/utils/error";
import { useLogin } from "../api/hooks";
import LoginForm, { LoginFormValues } from "./Forms/Login";
import RegisterForm, { RegisterFormInputs } from "./Forms/Register";

export default function LoginModal() {
  const toast = useToast();
  const { modalState, open, onClose } = useLoginModal();
  const [tabIndex, setTabIndex] = useState(0);
  const [loginError, setLoginError] = useState("");
  const [registerError, setRegisterError] = useState("");
  const authInputRef = useRef<HTMLInputElement | null>(null);
  const { mutateAsync: loginAsync } = useLogin();

  const handleLogin = async (values: LoginFormValues) => {
    try {
      await loginAsync(values);
      toast({
        status: "success",
        description: "You have logged in successfully. ",
      });
      onClose();
    } catch (err) {
      setLoginError(getErrorMessage(err));
    }
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
      setRegisterError(getErrorMessage(err));
    }
  };

  const renderAlertMessage = (err: string, onClose: () => void) => {
    if (!err) return null;
    return (
      <Alert status="error">
        <AlertIcon />
        <AlertDescription>{err}</AlertDescription>
        <CloseButton
          onClick={onClose}
          position="absolute"
          right="8px"
          top="8px"
        />
      </Alert>
    );
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
                {renderAlertMessage(loginError, () => setLoginError(""))}
                <LoginForm initialRef={authInputRef} onSubmit={handleLogin} />
              </TabPanel>
              <TabPanel>
                {renderAlertMessage(registerError, () => setRegisterError(""))}
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
