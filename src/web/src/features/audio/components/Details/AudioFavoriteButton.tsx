import { IconButton } from "@chakra-ui/button";
import React from "react";
import { FaHeart } from "react-icons/fa";
import { useUser } from "~/features/user/hooks";
import { useFavoriteAudio } from "../../hooks";
import { AudioId } from "../../api/types";

interface AudioFavoriteButtonProps {
  audioId: AudioId;
}

export default function AudioFavoriteButton({
  audioId,
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
      size="lg"
    />
  );
}
