import {
  Box,
  CloseButton,
  Flex,
  Heading,
  List,
  ListItem,
  useColorModeValue,
} from "@chakra-ui/react";
import React, { useContext } from "react";
import { AudioPlayerContext } from "~/contexts/AudioPlayerContext";

interface AudioQueueProps {
  isOpen: boolean;
  onClose: () => void;
}

export default function AudioQueue(props: AudioQueueProps) {
  const { isOpen, onClose } = props;
  const { state, dispatch } = useContext(AudioPlayerContext);
  const { queue, playIndex } = state;
  const bgColor = useColorModeValue("white", "gray.800");
  const hoverColor = useColorModeValue("gray.300", "gray.900");

  return (
    <Box
      overflow="hidden"
      position="fixed"
      right={{ base: "0px", md: "33px" }}
      bottom="65px"
      display={isOpen ? "block" : "none"}
      width={{ base: "100%", md: "480px" }}
      height={{ base: "750px", md: "410px" }}
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
        <List>
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
              lineHeight="40px"
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
              <Box flex="1">{audio.title}</Box>
              <Flex justifyContent="flex-end">
                {playIndex !== index && (
                  <CloseButton
                    onClick={(e) => {
                      e.stopPropagation();
                      dispatch({ type: "REMOVE_FROM_QUEUE", payload: index });
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
