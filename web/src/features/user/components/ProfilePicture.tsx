import React, { useState } from "react";
import NextImage from "next/image";
import PictureController from "~/components/Picture";
import PictureContainer from "~/components/Picture/PictureContainer";
import { useAddUserPicture } from "../hooks";

interface ProfilePictureProps {
  username: string;
  pictureSrc: string;
  canModify?: boolean;
}

export default function ProfilePicture({
  username,
  pictureSrc,
  canModify = false,
}: ProfilePictureProps) {
  const [picture, setPicture] = useState(pictureSrc);

  const {
    mutateAsync: addPictureAsync,
    isLoading: isAddingPicture,
  } = useAddUserPicture(username);

  return (
    <PictureController
      title={username}
      src={picture}
      onChange={async (croppedData) => {
        const data = await addPictureAsync(croppedData);
        setPicture(data.image);
      }}
      isUploading={isAddingPicture}
      canEdit={canModify}
    >
      <PictureContainer width={200}>
        {pictureSrc && (
          <NextImage
            src={pictureSrc}
            layout="fill"
            objectFit="cover"
            loading="eager"
            alt={username}
          />
        )}
      </PictureContainer>
    </PictureController>
  );
}
