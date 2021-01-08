import { Box, Flex, Heading, Text } from "@chakra-ui/react";
import { useRouter } from "next/router";
import React, { useEffect } from "react";
import { relativeDate } from "~/utils/time";
import Link from "../Shared/Link";

interface AudioMetaProps {
  title: string;
  description: string;
  created: string | Date;
  username: string;
}

const AudioDetails: React.FC<AudioMetaProps> = (props) => {
  const router = useRouter();

  useEffect(() => {
    router.prefetch(`/users/${props.username}`);
  }, [router]);

  return (
    <Flex marginBottom={4}>
      <Box flex="1">
        <Flex alignItems="center">
          <Link href={`/users/${props.username}`}>
            <Text fontSize="sm">{props.username}</Text>
          </Link>
        </Flex>
        <Heading as="h1" fontSize="3xl" paddingY="2">
          {props.title}
        </Heading>
        <Text fontSize="sm">{props.description}</Text>
        <Text fontSize="xs" as="i">
          Uploaded {relativeDate(props.created)}
        </Text>
      </Box>
    </Flex>
  );
};

export default AudioDetails;
