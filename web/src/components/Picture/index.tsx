import { Box, useColorModeValue } from "@chakra-ui/react";
import React from "react";
import NextImage from "next/image";

export interface PictureProps {
  source: string;
  imageSize: number;
  isLazy?: boolean;
  onClick?: () => void;
}

export default function Picture(props: PictureProps) {
  const { source, imageSize, isLazy = false, onClick } = props;
  const color1 = useColorModeValue("gray.500", "gray.900");
  const color2 = useColorModeValue("gray.400", "gray.800");

  return (
    <Box
      bgGradient={`linear(to-r, ${color1}, ${color2})`}
      width={imageSize}
      borderRightWidth={1}
      _after={{
        content: '""',
        display: "block",
        paddingBottom: "100%",
      }}
      position="relative"
      onClick={onClick}
      cursor={onClick && "pointer"}
    >
      {source && (
        <NextImage
          src={source}
          layout="fill"
          objectFit="cover"
          loading={isLazy ? "lazy" : "eager"}
        />
      )}
    </Box>
  );
}
