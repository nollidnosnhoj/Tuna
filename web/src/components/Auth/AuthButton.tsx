import { Button, ButtonProps, useDisclosure } from "@chakra-ui/react";
import React from "react";
import AuthModal from "./AuthModal";

export type AuthCommonType = "login" | "register";

interface AuthButtonProps extends ButtonProps {
  authType: AuthCommonType;
}

const AuthButton: React.FC<AuthButtonProps> = ({ authType, ...props }) => {
  const { isOpen, onOpen, onClose } = useDisclosure();

  return (
    <React.Fragment>
      <Button
        colorScheme={authType === "register" ? "primary" : "gray"}
        variant="ghost"
        onClick={onOpen}
        {...props}
      >
        {authType === "login" && "LOGIN"}
        {authType === "register" && "REGISTER"}
      </Button>
      <AuthModal type={authType} isOpen={isOpen} onClose={onClose} />
    </React.Fragment>
  );
};

export default AuthButton;
