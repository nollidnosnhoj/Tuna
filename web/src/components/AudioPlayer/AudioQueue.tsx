import {
  Box,
  CloseButton,
  Flex,
  Heading,
  List,
  ListItem,
  useColorModeValue,
} from "@chakra-ui/react";
import React from "react";
import useAudioQueue from "~/hooks/useAudioQueue";

interface AudioQueueProps {
  isOpen: boolean;
  onClose: () => void;
}

export default function AudioQueue(props: AudioQueueProps) {
  const { isOpen, onClose } = props;
  const { audioList, playIndex, removeFromQueue, goToIndex } = useAudioQueue();
  const bgColor = useColorModeValue("white", "gray.800");
  const hoverColor = useColorModeValue("gray.300", "gray.900");

  return (
    <Box
      overflow="hidden"
      position="fixed"
      right="33px"
      bottom="65px"
      display={isOpen ? "block" : "none"}
      width="480px"
      height="410px"
      borderWidth="1px"
      borderBottomWidth="0"
      bgColor={bgColor}
    >
      <Flex
        paddingX={4}
        paddingY={4}
        alignItems="center"
        borderBottomWidth="1px"
      >
        <Heading as="h2" size="md" flex="1">
          Queue
        </Heading>
        <CloseButton onClick={onClose} />
      </Flex>
      <Box overflowX="hidden" overflowY="auto" height="359px">
        <List>
          {audioList.length === 0 && (
            <ListItem
              lineHeight="40px"
              display="flex"
              alignItems="center"
              cursor="pointer"
              paddingX={4}
              paddingY={2}
              width="100%"
            >
              No audio queued.
            </ListItem>
          )}
          {audioList.map((audio, index) => (
            <ListItem
              key={index}
              lineHeight="40px"
              display="flex"
              alignItems="center"
              cursor="pointer"
              paddingX={4}
              paddingY={2}
              onClick={() => {
                playIndex !== index && goToIndex(index);
              }}
              bgColor={playIndex === index ? hoverColor : undefined}
              _hover={{ bgColor: hoverColor }}
              width="100%"
            >
              <Box flex="1">{audio.title}</Box>
              <Flex justifyContent="flex-end">
                {playIndex !== index && (
                  <CloseButton
                    onClick={(e) => {
                      e.stopPropagation();
                      removeFromQueue(index);
                    }}
                  />
                )}
              </Flex>
            </ListItem>
          ))}
        </List>
      </Box>
    </Box>
  );
}
