import { Button } from "@chakra-ui/react";
import React from "react";
import { useUser } from "~/features/user/hooks";
import { useFollow } from "../../../hooks";

interface ProfileFollowButtonProps {
  profileId: number;
}

export default function ProfileFollowButton({
  profileId,
}: ProfileFollowButtonProps) {
  const { user } = useUser();

  const { isFollowing, follow } = useFollow(profileId);

  if (!user || user.id === profileId) {
    return null;
  }

  return (
    <Button
      colorScheme="primary"
      variant={isFollowing ? "solid" : "outline"}
      disabled={isFollowing === undefined}
      paddingX={12}
      onClick={() => follow()}
    >
      {isFollowing ? "Followed" : "Follow"}
    </Button>
  );
}
