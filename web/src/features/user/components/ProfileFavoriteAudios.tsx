import { Box } from "@chakra-ui/layout";
import { Button } from "@chakra-ui/react";
import React from "react";
import NextLink from "next/link";
import AudioList from "~/features/audio/components/List";
import { useGetUserFavoriteAudios } from "../hooks";

interface ProfileLatestAudiosProps {
  username: string;
}

export default function ProfileFavoriteAudios({
  username,
}: ProfileLatestAudiosProps) {
  const { items: latestFavoriteAudios } = useGetUserFavoriteAudios(
    username,
    { size: 5 },
    { staleTime: 1000 * 60 * 5 }
  );

  return (
    <Box>
      <AudioList
        audios={latestFavoriteAudios}
        notFoundContent={<p>No favorite audios.</p>}
        defaultLayout="list"
        hideLayoutToggle
      />
      {latestFavoriteAudios.length > 0 && (
        <NextLink href={`/users/${username}/favorite/audios`}>
          <Button width="100%">View More</Button>
        </NextLink>
      )}
    </Box>
  );
}
