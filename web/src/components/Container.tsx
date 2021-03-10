import { BoxProps, Container, Stack } from "@chakra-ui/react";

const AudiochanContainer: React.FC<BoxProps> = ({ children, ...props }) => {
  return (
    <Container as={Stack} maxW="8xl" px={{ base: "2", md: "6" }} {...props}>
      {children}
    </Container>
  );
};

export default AudiochanContainer;
