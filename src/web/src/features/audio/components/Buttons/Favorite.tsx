import { IconButton } from "@chakra-ui/button";
import React from "react";
import { FaHeart } from "react-icons/fa";
import { useUser } from "~/features/user/hooks";
import { useFavoriteAudio } from "../../api/hooks";
import { AudioId } from "../../api/types";
import { ButtonProps } from "@chakra-ui/react";

interface AudioFavoriteButtonProps extends ButtonProps {
  audioId: AudioId;
}

export default function AudioFavoriteButton({
  audioId,
  ...buttonProps
}: AudioFavoriteButtonProps) {
  const { user } = useUser();
  const { isFavorite, favorite, isLoading } = useFavoriteAudio(audioId);

  if (!user) {
    return null;
  }

  return (
    <IconButton
      aria-label={isFavorite ? "Unfavorite" : "Favorite"}
      icon={<FaHeart />}
      colorScheme="primary"
      variant="ghost"
      opacity={isFavorite ? 1 : 0.5}
      isRound
      onClick={favorite}
      isLoading={isLoading}
      {...buttonProps}
    />
  );
}