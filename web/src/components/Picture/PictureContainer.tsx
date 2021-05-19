import { useColorModeValue, Box, BoxProps } from "@chakra-ui/react";
import React from "react";

type PictureContainerProps = BoxProps;

const PictureContainer: React.FC<PictureContainerProps> = ({
  children,
  ...boxProps
}) => {
  const [color1, color2] = useColorModeValue<
    [string, string],
    [string, string]
  >(["gray.500", "gray.400"], ["gray.900", "gray.800"]);

  return (
    <Box
      bgGradient={`linear(to-r, ${color1}, ${color2})`}
      _after={{
        content: '""',
        display: "block",
        paddingBottom: "100%",
      }}
      position="relative"
      {...boxProps}
    >
      {children}
    </Box>
  );
};

export default PictureContainer;
