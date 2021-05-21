import { Box } from "@chakra-ui/layout";
import { Button } from "@chakra-ui/react";
import React from "react";
import NextLink from "next/link";
import AudioList from "~/features/audio/components/List";
import { useGetUserAudios } from "../hooks";

interface ProfileLatestAudiosProps {
  username: string;
}

export default function ProfileLatestAudios({
  username,
}: ProfileLatestAudiosProps) {
  const { items: latestAudios } = useGetUserAudios(
    username,
    { size: 5 },
    { staleTime: 1000 }
  );

  return (
    <Box>
      <AudioList
        audios={latestAudios}
        notFoundContent={<p>No uploads.</p>}
        defaultLayout="list"
        hideLayoutToggle
      />
      {latestAudios.length > 0 && (
        <NextLink href={`/users/${username}/audios`}>
          <Button width="100%">View More</Button>
        </NextLink>
      )}
    </Box>
  );
}
