import { BoxProps, Container, Stack } from "@chakra-ui/react";

const AudiochanContainer: React.FC<BoxProps> = ({ children, ...props }) => {
  return (
    <Container as={Stack} maxW="1200px" {...props}>
      {children}
    </Container>
  );
};

export default AudiochanContainer;
