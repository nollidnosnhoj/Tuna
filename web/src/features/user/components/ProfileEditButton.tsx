import { Button } from "@chakra-ui/react";
import React from "react";
import { useUser } from "~/lib/hooks/useUser";
import { Profile } from "../types";

interface ProfileEditButtonProps {
  profile: Profile;
}

export default function ProfileEditButton({ profile }: ProfileEditButtonProps) {
  const { user } = useUser();

  if (!user || user.id !== profile.id) {
    return null;
  }

  return (
    <React.Fragment>
      <Button size="sm">Edit Profile</Button>
    </React.Fragment>
  );
}
