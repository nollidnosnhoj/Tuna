import { Alert, AlertDescription, AlertIcon } from "@chakra-ui/alert";
import { Divider, Heading, Stack } from "@chakra-ui/layout";
import { chakra } from "@chakra-ui/system";
import React, { PropsWithChildren } from "react";

interface AuthContainerProps {
  headingText: string;
  subHeader?: React.ReactElement;
  footer?: React.ReactElement;
  error: string;
}

export default function AuthContainer(
  props: PropsWithChildren<AuthContainerProps>
) {
  return (
    <chakra.div display="flex" justifyContent="center">
      <chakra.div maxWidth="500px" width="80%">
        <Stack direction="column" spacing={2}>
          <chakra.div display="flex" justifyContent="center">
            <Heading size="lg">{props.headingText}</Heading>
          </chakra.div>
          {props.subHeader && (
            <chakra.div marginY={2}>{props.subHeader}</chakra.div>
          )}
          {!!props.error && (
            <chakra.div marginY={2}>
              <Alert status="error">
                <AlertIcon />
                <AlertDescription>{props.error}</AlertDescription>
              </Alert>
            </chakra.div>
          )}
          <chakra.div paddingY={2}>{props.children}</chakra.div>
          {props.footer && (
            <React.Fragment>
              <Divider />
              <chakra.div paddingY={4} textAlign="center">
                {props.footer}
              </chakra.div>
            </React.Fragment>
          )}
        </Stack>
      </chakra.div>
    </chakra.div>
  );
}
