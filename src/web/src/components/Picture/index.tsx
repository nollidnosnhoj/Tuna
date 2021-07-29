import { Box, Button, useDisclosure } from "@chakra-ui/react";
import NextImage from "next/image";
import React, { useContext, useEffect, useMemo, useState } from "react";
import { useDropzone } from "react-dropzone";
import SETTINGS from "~/lib/config";
import { errorToast, toast } from "~/utils";
import PictureContainer from "./PictureContainer";
import PictureCropModal from "./PictureCropModal";
import PictureModal from "./PictureModal";

type PictureContextType = {
  canEdit: boolean;
  onUpload: (imageData: string) => Promise<void>;
  isUploading: boolean;
};

interface PictureProps {
  src: string;
  title: string;
  onChange: (data: string) => Promise<void>;
  isUploading: boolean;
  canEdit?: boolean;
}

const PictureContext = React.createContext<PictureContextType>(
  {} as PictureContextType
);

export default function PictureController(props: PictureProps) {
  const { src, title, onChange, isUploading, canEdit = false } = props;

  const {
    isOpen: isPictureModalOpen,
    onOpen: onPictureModalOpen,
    onClose: onPictureModalClose,
  } = useDisclosure();

  const {
    isOpen: isCropModalOpen,
    onOpen: onCropModalOpen,
    onClose: onCropModalClose,
  } = useDisclosure();

  const [file, setFile] = useState<File | null>(null);

  const { open, getInputProps } = useDropzone({
    accept: SETTINGS.UPLOAD.IMAGE.accept,
    maxSize: SETTINGS.UPLOAD.IMAGE.maxSize,
    multiple: false,
    onDropAccepted: ([file]) => {
      setFile(file);
    },
    onDropRejected: ([fileRejection]) => {
      /** Display error toasts */
      fileRejection.errors.forEach((err) => {
        toast("error", {
          title: "Invalid Image",
          description: err.message,
        });
      });
    },
  });

  const onUpload = async (imageData: string) => {
    try {
      await onChange(imageData);
    } catch (err) {
      errorToast(err);
    }
  };

  useEffect(() => {
    if (file) {
      onCropModalOpen();
    }
  }, [file]);

  const values = useMemo(
    () => ({
      onUpload,
      canEdit,
      isUploading,
    }),
    [onUpload, canEdit]
  );

  return (
    <PictureContext.Provider value={values}>
      <input {...getInputProps()} />
      <Box
        onClick={onPictureModalOpen}
        cursor="pointer"
        display="flex"
        justifyContent="center"
        position="relative"
      >
        <PictureContainer width={200}>
          {src && (
            <NextImage
              src={src}
              layout="fill"
              objectFit="cover"
              loading="eager"
              alt={title}
            />
          )}
        </PictureContainer>
        {canEdit && (
          <Button
            colorScheme="primary"
            size="sm"
            position="absolute"
            bottom="5%"
            paddingX={4}
            onClick={(e) => {
              e.stopPropagation();
              open();
            }}
          >
            Upload
          </Button>
        )}
      </Box>
      {file && (
        <PictureCropModal
          isOpen={isCropModalOpen}
          onClose={onCropModalClose}
          file={file}
        />
      )}
      <PictureModal
        title={title}
        src={src}
        isOpen={isPictureModalOpen}
        onClose={onPictureModalClose}
      />
    </PictureContext.Provider>
  );
}

export function usePicture() {
  const context = useContext(PictureContext);
  if (!context) {
    throw new Error("PictureContext was not found.");
  }
  return context;
}
