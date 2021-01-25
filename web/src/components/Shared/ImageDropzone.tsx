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
  onChange: (file: File) => void;
  buttonWidth?: string | number;
}

const ImageDropzone: React.FC<ImageDropzoneProps> = ({
  name,
  buttonWidth,
  onChange,
}) => {
  const [croppedImage, setCroppedImage] = useState<string>(null);

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
      {croppedImage && (
        <Flex justifyContent="center" marginBottom={4}>
          <Image boxSize="300px" objectFit="cover" src={croppedImage} />
        </Flex>
      )}
      <input {...getInputProps({ name })} />
      <Button width={buttonWidth} onClick={open}>
        {croppedImage ? "Replace Image" : "Upload Image"}
      </Button>
      <ImageCropModal
        file={acceptedFiles[0]}
        isOpen={isImageCropModalOpen}
        onClose={closeImageCropModal}
        onCropped={(croppedFile) => {
          setCroppedImage(window.URL.createObjectURL(croppedFile));
          onChange(croppedFile);
        }}
      />
    </Box>
  );
};

export default ImageDropzone;
