import { Alert, AlertDescription, AlertIcon } from "@chakra-ui/alert";
import { Divider, Heading } from "@chakra-ui/layout";
import { chakra } from "@chakra-ui/system";
import React, { PropsWithChildren } from "react";

interface AuthContainerProps {
  headingText: string;
  footer: React.ReactElement;
  error: string;
}

export default function AuthContainer(
  props: PropsWithChildren<AuthContainerProps>
) {
  return (
    <chakra.div display="flex" justifyContent="center">
      <chakra.div maxWidth="500px" width="80%">
        <chakra.div display="flex" justifyContent="center" marginY={4}>
          <Heading>{props.headingText}</Heading>
        </chakra.div>
        {!!props.error && (
          <chakra.div marginY={2}>
            <Alert status="error">
              <AlertIcon />
              <AlertDescription>{props.error}</AlertDescription>
            </Alert>
          </chakra.div>
        )}
        <chakra.div paddingY={2}>{props.children}</chakra.div>
        <Divider marginY={4} />
        <chakra.div textAlign="center">{props.footer}</chakra.div>
      </chakra.div>
    </chakra.div>
  );
}
