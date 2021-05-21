import { useDisclosure } from "@chakra-ui/react";
import Head from "next/head";
import React, { PropsWithChildren, useState, useMemo } from "react";
import AuthModal from "~/features/auth/components/AuthModal";
import { PageContextType, PageContext } from "~/lib/contexts";

interface PageProviderProps {
  title?: string;
}

export function PageProvider(props: PropsWithChildren<PageProviderProps>) {
  const [authTab, setAuthTab] = useState(0);
  const { isOpen, onClose, onOpen } = useDisclosure();

  const openLogin = () => {
    setAuthTab(0);
    onOpen();
  };

  const openRegister = () => {
    setAuthTab(1);
    onOpen();
  };

  const values = useMemo<PageContextType>(
    () => ({
      openLogin,
      openRegister,
    }),
    [openLogin, openRegister]
  );

  return (
    <React.Fragment>
      <Head>
        <title>{props.title}</title>
      </Head>
      <PageContext.Provider value={values}>
        {props.children}
        <AuthModal
          isOpen={isOpen}
          onClose={onClose}
          tabIndex={authTab}
          onTabChange={(index) => setAuthTab(index)}
        />
      </PageContext.Provider>
    </React.Fragment>
  );
}
