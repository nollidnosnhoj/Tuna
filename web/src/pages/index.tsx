import {
  Box,
  Button,
  chakra,
  Stack,
  useColorModeValue,
} from "@chakra-ui/react";
import { useRouter } from "next/router";
import React, { useCallback } from "react";
import Page from "~/components/Page";
import { useUser } from "~/components/providers/UserProvider";

const Index = () => {
  const router = useRouter();
  const { isLoggedIn } = useUser();
  const handleUploadButton = useCallback(() => {
    if (isLoggedIn) {
      router.push("/upload");
    } else {
      router.push("/register");
    }
  }, [isLoggedIn]);
  return (
    <Page title="Audiochan | Listen and Share Your Music">
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
                    Welcome to{" "}
                  </chakra.span>
                  <chakra.span
                    display={{ base: "block", xl: "inline" }}
                    color={useColorModeValue("primary.600", "primary.400")}
                  >
                    Audiochan!
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
                  Your repository for free, non-copyright music uploaded by
                  independent musicians.
                </chakra.p>
                <Stack
                  direction={{ base: "column", sm: "column", md: "row" }}
                  mb={{ base: 4, md: 8 }}
                  spacing={{ base: 4, md: 2 }}
                  justifyContent="center"
                >
                  <Box>
                    <Button
                      fontSize={{ base: "md", md: "lg" }}
                      rounded="md"
                      color="white"
                      bg="primary.600"
                      _hover={{ bg: "primary.700" }}
                      onClick={handleUploadButton}
                      // px={{ base: 8, md: 10 }}
                      // py={{ base: 3, md: 4 }}
                    >
                      Start Uploading
                    </Button>
                  </Box>
                  <Box mt={[3, 0]} ml={[null, 3]}>
                    <Button
                      // px={{ base: 8, md: 10 }}
                      // py={{ base: 3, md: 4 }}
                      border="solid 1px transparent"
                      fontSize={{ base: "md", md: "lg" }}
                      rounded="md"
                      color="primary.700"
                      bg="primary.100"
                      _hover={{ bg: "primary.200" }}
                    >
                      Start Listening
                    </Button>
                  </Box>
                </Stack>
              </Box>
            </Box>
          </Box>
        </Box>
      </Box>
    </Page>
  );
};

export default Index;
