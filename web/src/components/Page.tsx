import { useDisclosure } from "@chakra-ui/react";
import Head from "next/head";
import React, {
  createContext,
  PropsWithChildren,
  useContext,
  useMemo,
  useState,
} from "react";
import Header from "~/components/ui/Header";
import Container from "~/components/ui/Container";
import AuthModal from "~/features/auth/components/AuthModal";

type PageContextType = {
  openLogin: () => void;
  openRegister: () => void;
};

interface PageProviderProps {
  title?: string;
}

const PageContext = createContext<PageContextType>({} as PageContextType);

export const usePage = () => {
  const ctx = useContext(PageContext);
  if (!ctx) throw new Error("Cannot use PageContext");
  return ctx;
};

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

interface PageProps {
  title?: string;
  removeHeader?: boolean;
}

const Page: React.FC<PageProps> = ({
  title = "Audiochan",
  removeHeader = false,
  children,
  ...props
}) => {
  return (
    <PageProvider title={title}>
      {!removeHeader && <Header />}
      <Container mt="120px" mb="100px" {...props}>
        {children}
      </Container>
    </PageProvider>
  );
};

export default Page;
