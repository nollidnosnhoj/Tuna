import React from "react";
import { Box, BoxProps } from "@chakra-ui/react";

const Container = (props: BoxProps) => {
  return (
    <Box
      w="full"
      pb="12"
      pt="3"
      mx="auto"
      px={{ base: "2", md: "6" }}
      {...props}
    >
      {props.children}
    </Box>
  );
};

export default Container;
