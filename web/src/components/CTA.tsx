import {
  Box,
  chakra,
  useColorModeValue,
  Stack,
  Button,
} from "@chakra-ui/react";
import NextLink from "next/link";
import React from "react";
import { FaRandom, FaCloudUploadAlt } from "react-icons/fa";
import { MdLibraryMusic } from "react-icons/md";
import { useAuth } from "~/contexts/AuthContext";

export function HomepageCTA() {
  const { isLoggedIn } = useAuth();
  return (
    <Box pos="relative" overflow="hidden">
      <Box maxW="7xl" mx="auto">
        <Box
          pos="relative"
          pb={{ base: 8, sm: 16, md: 20, lg: 28, xl: 32 }}
          w="full"
          border="solid 1px transparent"
        >
          <Box
            mx="auto"
            maxW={{ base: "7xl" }}
            px={{ base: 4, sm: 6, lg: 8 }}
            mt={{ base: 12, md: 16, lg: 20, xl: 28 }}
          >
            <Box
              textAlign="center"
              w={{ base: "full", md: 11 / 12, xl: 8 / 12 }}
              mx="auto"
            >
              <chakra.h1
                fontSize={{ base: "4xl", sm: "5xl", md: "6xl" }}
                letterSpacing="tight"
                lineHeight="short"
                fontWeight="extrabold"
                color={useColorModeValue("gray.900", "white")}
              >
                <chakra.span display={{ base: "block", xl: "inline" }}>
                  Start uploading and sharing your{" "}
                </chakra.span>
                <chakra.span
                  display={{ base: "block", xl: "inline" }}
                  color={useColorModeValue("primary.600", "primary.400")}
                >
                  audio
                </chakra.span>
              </chakra.h1>
              <chakra.p
                mt={{ base: 3, sm: 5, md: 5 }}
                mx={{ sm: "auto", lg: 0 }}
                mb={6}
                fontSize={{ base: "lg", md: "xl" }}
                color="gray.500"
                lineHeight="base"
              >
                Audiochan allows you to easily upload audio to share online. You
                can also view publicly available audio to share or save.
              </chakra.p>
              <Stack
                direction={{ base: "column", sm: "column", md: "row" }}
                mb={{ base: 4, md: 8 }}
                spacing={{ base: 4, md: 2 }}
                justifyContent="center"
              >
                <NextLink href="/audios">
                  <Button
                    leftIcon={<MdLibraryMusic />}
                    fontSize={{ base: "md", md: "lg" }}
                    px={{ base: 8, md: 10 }}
                    py={{ base: 3, md: 4 }}
                  >
                    Browse
                  </Button>
                </NextLink>
                <NextLink href="#">
                  <Button
                    leftIcon={<FaRandom />}
                    fontSize={{ base: "md", md: "lg" }}
                    px={{ base: 8, md: 10 }}
                    py={{ base: 3, md: 4 }}
                  >
                    Random
                  </Button>
                </NextLink>
                <NextLink href={isLoggedIn ? "/upload" : "/auth/login"}>
                  <Button
                    leftIcon={<FaCloudUploadAlt />}
                    colorScheme="primary"
                    fontSize={{ base: "md", md: "lg" }}
                    px={{ base: 8, md: 10 }}
                    py={{ base: 3, md: 4 }}
                  >
                    Upload
                  </Button>
                </NextLink>
              </Stack>
            </Box>
          </Box>
        </Box>
      </Box>
    </Box>
  );
}
