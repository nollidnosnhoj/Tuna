import {
  Box,
  chakra,
  CloseButton,
  Flex,
  Heading,
  List,
  ListItem,
  useColorModeValue,
} from "@chakra-ui/react";
import React from "react";
import { useAudioPlayer } from "~/lib/hooks/useAudioPlayer";

interface AudioQueueProps {
  isOpen: boolean;
  onClose: () => void;
}

export default function AudioQueuePanel(props: AudioQueueProps) {
  const { isOpen, onClose } = props;
  const { state, dispatch } = useAudioPlayer();
  const { queue, playIndex } = state;
  const bgColor = useColorModeValue("white", "gray.800");
  const hoverColor = useColorModeValue("gray.300", "gray.900");

  return (
    <Box
      overflow="hidden"
      position="fixed"
      right={{ base: "0px", md: "33px" }}
      bottom="100px"
      display={isOpen ? "block" : "none"}
      width={{ base: "100%", md: "480px" }}
      height={{ base: "750px", md: "425px" }}
      borderWidth="1px"
      borderBottomWidth="0"
      bgColor={bgColor}
      boxShadow="base"
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
        <List fontSize="sm">
          {queue.length === 0 && (
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
          {queue.map((audio, index) => (
            <ListItem
              key={index}
              lineHeight="30px"
              display="flex"
              alignItems="center"
              cursor="pointer"
              paddingX={4}
              paddingY={2}
              onClick={() => {
                playIndex !== index &&
                  dispatch({ type: "SET_PLAY_INDEX", payload: index });
              }}
              bgColor={playIndex === index ? hoverColor : undefined}
              _hover={{ bgColor: hoverColor }}
              width="100%"
            >
              <Box flex="2">
                <chakra.strong>{audio.title}</chakra.strong>
              </Box>
              <Box flex="2">
                <chakra.span>{audio.artist}</chakra.span>
              </Box>
              <Flex flex="1" justifyContent="flex-end" width="50px">
                <CloseButton
                  onClick={(e) => {
                    e.stopPropagation();
                    dispatch({ type: "REMOVE_FROM_QUEUE", payload: index });
                  }}
                />
              </Flex>
            </ListItem>
          ))}
        </List>
      </Box>
    </Box>
  );
}
