import { DeleteIcon } from "@chakra-ui/icons";
import {
  Box,
  Button,
  ButtonGroup,
  IconButton,
  useDisclosure,
} from "@chakra-ui/react";
import NextImage from "next/image";
import React, { useContext, useMemo, useState } from "react";
import { useDropzone } from "react-dropzone";
import SETTINGS from "~/lib/config";
import { errorToast, toast } from "~/utils";
import PictureContainer from "./PictureContainer";
import PictureCropModal from "./PictureCropModal";
import PictureModal from "./PictureModal";

type PictureContextType = {
  canEdit: boolean;
  onUpload: (imageData: string) => Promise<void>;
  isMutating: boolean;
};

interface PictureProps {
  src: string;
  title: string;
  onChange: (data: string) => Promise<void>;
  onRemove: () => Promise<void>;
  isMutating: boolean;
  canEdit?: boolean;
  width?: number;
}

const PictureContext = React.createContext<PictureContextType>(
  {} as PictureContextType
);

export default function PictureController(props: PictureProps) {
  const {
    src,
    title,
    onChange,
    onRemove,
    isMutating,
    canEdit = false,
    width = 300,
  } = props;

  const {
    isOpen: isPictureModalOpen,
    onOpen: onPictureModalOpen,
    onClose: onPictureModalClose,
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

  const onCloseCropModal = () => {
    setFile(null);
  };

  const values = useMemo(
    () => ({
      onUpload,
      canEdit,
      isMutating,
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
        position="relative"
      >
        <PictureContainer width={width}>
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
          <ButtonGroup
            position="absolute"
            bottom="5%"
            left="50%"
            transform="translate(-50%, -5%)"
            size="sm"
            isAttached
            isDisabled={isMutating}
          >
            <Button
              colorScheme="primary"
              paddingX={4}
              onClick={(e) => {
                e.stopPropagation();
                open();
              }}
            >
              Upload
            </Button>
            <IconButton
              colorScheme="primary"
              aria-label="Remove Image"
              icon={<DeleteIcon />}
              onClick={async (e) => {
                e.stopPropagation();
                if (!confirm("Are you sure you want to remove image?")) return;
                await onRemove();
              }}
            />
          </ButtonGroup>
        )}
      </Box>
      <PictureCropModal
        isOpen={!!file}
        onClose={onCloseCropModal}
        file={file}
      />
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
