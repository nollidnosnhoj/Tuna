import React, { useState } from "react";
import NextImage from "next/image";
import PictureController from "~/components/Picture";
import PictureContainer from "~/components/Picture/PictureContainer";
import { useAddAudioPicture } from "../hooks";

interface AudioPictureProps {
  audioId: string;
  pictureTitle: string;
  pictureSrc: string;
  canModify?: boolean;
}

export default function AudioPicture({
  audioId,
  pictureTitle,
  canModify = false,
  pictureSrc,
}: AudioPictureProps) {
  const [picture, setPicture] = useState(pictureSrc);

  const {
    mutateAsync: addPictureAsync,
    isLoading: isAddingPicture,
  } = useAddAudioPicture(audioId);

  return (
    <PictureController
      title={pictureTitle}
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
            alt={pictureTitle}
          />
        )}
      </PictureContainer>
    </PictureController>
  );
}
