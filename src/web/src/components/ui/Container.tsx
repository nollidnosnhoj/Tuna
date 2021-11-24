import { Box, BoxProps } from "@chakra-ui/layout";
import React, { PropsWithChildren } from "react";

export default function Container(props: PropsWithChildren<BoxProps>) {
  const { children, ...boxProps } = props;
  return (
    <Box ml={{ base: 0, md: 60 }} p="4" {...boxProps}>
      {children}
    </Box>
  );
}
