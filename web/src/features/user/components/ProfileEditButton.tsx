import { Button } from "@chakra-ui/react";
import React from "react";
import { useUser } from "~/features/user/hooks/useUser";

interface ProfileEditButtonProps {
  profileId: string;
}

export default function ProfileEditButton({
  profileId,
}: ProfileEditButtonProps) {
  const { user } = useUser();

  if (!user || user.id !== profileId) {
    return null;
  }

  return (
    <React.Fragment>
      <Button size="sm">Edit Profile</Button>
    </React.Fragment>
  );
}
