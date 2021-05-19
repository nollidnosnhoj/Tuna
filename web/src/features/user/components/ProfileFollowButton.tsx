import { Button } from "@chakra-ui/react";
import React from "react";
import { useUser } from "~/lib/hooks/useUser";
import { useFollow } from "../hooks";
import { Profile } from "../types";

interface ProfileFollowButtonProps {
  profile: Profile;
}

export default function ProfileFollowButton({
  profile,
}: ProfileFollowButtonProps) {
  const { user } = useUser();

  const { isFollowing, follow } = useFollow(
    profile.username,
    profile.isFollowing
  );

  if (!user || user.id === profile.id) {
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
