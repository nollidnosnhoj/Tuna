import { Box, Button, Flex, Image, useDisclosure } from "@chakra-ui/react";
import React, { useState } from "react";
import { useDropzone } from "react-dropzone";
import dynamic from "next/dynamic";
import { errorToast } from "~/utils/toast";

const ImageCropModal = dynamic(
  () => import("~/components/Shared/ImageCropModal"),
  { ssr: false }
);

interface ImageDropzoneProps {
  name: string;
  onChange: (file: File) => Promise<void>;
  initialImage?: string;
  buttonWidth?: string | number;
  disabled?: boolean;
}

const ImageDropzone: React.FC<ImageDropzoneProps> = ({
  name,
  buttonWidth,
  initialImage,
  onChange,
  disabled = false,
}) => {
  const {
    isOpen: isImageCropModalOpen,
    onOpen: openImageCropModal,
    onClose: closeImageCropModal,
  } = useDisclosure();
  const { open, getInputProps, acceptedFiles } = useDropzone({
    accept: "image/*",
    maxSize: 2097152,
    multiple: false,
    onDropAccepted: () => {
      // open image crop modal
      openImageCropModal();
    },
    onDropRejected: ([fileRejection]) => {
      fileRejection.errors.forEach((err) => {
        errorToast({
          title: "Invalid Image",
          message: err.message,
        });
      });
    },
  });

  return (
    <Box>
      <input {...getInputProps({ name })} />
      <Button width={buttonWidth} onClick={open} disabled={disabled}>
        {initialImage ? "Replace Image" : "Upload Image"}
      </Button>
      <ImageCropModal
        file={acceptedFiles[0]}
        isOpen={isImageCropModalOpen}
        onClose={closeImageCropModal}
        onCropped={async (croppedFile) => {
          await onChange(croppedFile);
        }}
      />
    </Box>
  );
};

export default ImageDropzone;
