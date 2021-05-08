import {
  Box,
  Menu,
  MenuButton,
  IconButton,
  MenuList,
  MenuGroup,
  MenuItem,
  MenuDivider,
  Button,
  Modal,
  ModalBody,
  ModalCloseButton,
  ModalContent,
  ModalHeader,
  ModalOverlay,
  Tab,
  TabList,
  TabPanel,
  TabPanels,
  Tabs,
  useDisclosure,
} from "@chakra-ui/react";
import router, { useRouter } from "next/router";
import React, { useCallback, useContext, useRef } from "react";
import { FaUserAlt } from "react-icons/fa";
import NextLink from "next/link";
import {
  TabbedModalContext,
  TabbedModalProvider,
} from "~/contexts/TabbedModalContext";
import { useUser } from "~/contexts/UserContext";
import { useAuth } from "~/contexts/AuthContext";
import LoginForm from "~/features/auth/components/LoginForm";
import RegisterForm from "~/features/auth/components/RegisterForm";

const GuestSection: React.FC = () => {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const { index, setIndex } = useContext(TabbedModalContext);
  const { query, push: routerPush } = useRouter();
  const initialRef = useRef<HTMLInputElement | null>(null);

  const onOpenModalOnTabIndex = (idx: number) => {
    setIndex(idx).then(() => onOpen());
  };

  return (
    <>
      <Button
        size="md"
        colorScheme="gray"
        variant="ghost"
        textTransform="uppercase"
        onClick={() => onOpenModalOnTabIndex(0)}
      >
        Login
      </Button>
      <Button
        size="md"
        colorScheme="primary"
        textTransform="uppercase"
        onClick={() => onOpenModalOnTabIndex(1)}
      >
        Register
      </Button>
      <Modal initialFocusRef={initialRef} isOpen={isOpen} onClose={onClose}>
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>
            {index === 0 && "Login"}
            {index === 1 && "Sign Up"}
          </ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <Tabs
              isLazy
              isFitted
              index={index}
              onChange={(index) => setIndex(index)}
            >
              <TabList>
                <Tab>Login</Tab>
                <Tab>Sign Up</Tab>
              </TabList>
              <TabPanels>
                <TabPanel>
                  <LoginForm initialRef={initialRef} />
                </TabPanel>
                <TabPanel>
                  <RegisterForm initialRef={initialRef} />
                </TabPanel>
              </TabPanels>
            </Tabs>
          </ModalBody>
        </ModalContent>
      </Modal>
    </>
  );
};

const UserSection: React.FC = () => {
  const { user } = useUser();
  const { isLoggedIn } = useAuth();

  if (isLoggedIn) {
    return (
      <Box zIndex={4}>
        <Menu placement="bottom-end">
          <MenuButton
            as={IconButton}
            icon={<FaUserAlt />}
            variant="ghost"
            colorScheme="primary"
          >
            Profile
          </MenuButton>
          <MenuList>
            <MenuGroup title={user?.username}>
              <NextLink href={`/users/${user?.username}`}>
                <MenuItem>Profile</MenuItem>
              </NextLink>
              <NextLink href="/setting">
                <MenuItem>Settings</MenuItem>
              </NextLink>
            </MenuGroup>
            <MenuDivider />
            <MenuItem onClick={() => router.push("/auth/logout")}>
              Logout
            </MenuItem>
          </MenuList>
        </Menu>
      </Box>
    );
  }

  return (
    <TabbedModalProvider>
      <GuestSection />
    </TabbedModalProvider>
  );
};

export default UserSection;
