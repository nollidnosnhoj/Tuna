import {
  Box,
  Button,
  Modal,
  ModalBody,
  ModalCloseButton,
  ModalContent,
  ModalFooter,
  ModalHeader,
  ModalOverlay,
} from "@chakra-ui/react";
import React, { useEffect, useState } from "react";
import ReactCropper from "react-cropper";
import "cropperjs/dist/cropper.css";
import { usePicture } from "~/components/Picture";

interface PictureCropProps {
  isOpen: boolean;
  onClose: () => void;
  file: File | null;
}

export default function PictureCropModal({
  file,
  isOpen,
  onClose,
}: PictureCropProps) {
  const { onUpload, isMutating } = usePicture();
  const [imageData, setImageData] = useState("");
  const [cropper, setCropper] = useState<Cropper | null>(null);

  /** Load image from File */
  useEffect(() => {
    if (file) {
      const reader = new FileReader();
      reader.addEventListener("load", () => {
        setImageData(reader.result as string);
      });
      reader.readAsDataURL(file);
    } else {
      setImageData("");
      setCropper(null);
    }
  }, [file, setCropper]);

  const clear = () => {
    setImageData("");
    setCropper(null);
    onClose();
  };

  const handleCropAndUpload = async () => {
    if (cropper) {
      const canvasData: HTMLCanvasElement = cropper.getCroppedCanvas({
        fillColor: "white",
      });

      const imageDataRequest = canvasData.toDataURL(file?.type);

      await onUpload(imageDataRequest);
      clear();
    }
  };

  return (
    <React.Fragment>
      <Modal isOpen={isOpen} onClose={clear} size="xl">
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>Crop Image</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <Box>
              <ReactCropper
                src={imageData}
                aspectRatio={1}
                checkOrientation={false}
                initialAspectRatio={1}
                autoCrop
                onInitialized={(instance) => {
                  setCropper(instance);
                }}
                viewMode={1}
                cropBoxMovable={false}
                cropBoxResizable={false}
                dragMode="move"
                wheelZoomRatio={0.4}
                autoCropArea={1}
                width={500}
                height={500}
                responsive
              />
            </Box>
          </ModalBody>
          <ModalFooter>
            <Button
              isLoading={isMutating}
              loadingText="Uploading..."
              colorScheme="primary"
              onClick={handleCropAndUpload}
            >
              {"Crop & Upload"}
            </Button>
          </ModalFooter>
        </ModalContent>
      </Modal>
    </React.Fragment>
  );
}
