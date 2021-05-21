import React from "react";
import Header from "~/components/ui/Header";
import Container from "~/components/ui/Container";
import { PageProvider } from "./providers";

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
