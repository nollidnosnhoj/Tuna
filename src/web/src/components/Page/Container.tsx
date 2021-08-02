import { Box, BoxProps } from "@chakra-ui/react";
import { PropsWithChildren } from "react";

export default function Container({
  children,
  ...props
}: PropsWithChildren<BoxProps>) {
  return (
    <Box paddingY={4} paddingX={4} {...props}>
      {children}
    </Box>
  );
}
