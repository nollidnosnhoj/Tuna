import { Box, Flex, Image } from "@chakra-ui/react";
import { PropsWithChildren, useState } from "react";
import ImageDropzone from "~/components/Shared/ImageDropzone";
import { apiErrorToast } from "~/utils/toast";

interface AudioImageProps {
  name: string;
  disabled?: boolean;
  imageData?: string;
  canReplace?: boolean;
  onReplace?: (file: File) => Promise<void>;
}

export default function AudioImage({
  imageData,
  disabled = false,
  canReplace = false,
  onReplace,
}: PropsWithChildren<AudioImageProps>) {
  const [image, setImage] = useState<string>(imageData);

  const changeImage = async (file: File) => {
    try {
      if (onReplace) await onReplace(file);
      setImage(window.URL.createObjectURL(file));
    } catch (err) {
      apiErrorToast(err);
    }
  };

  return (
    <Box>
      <Flex justify="center" marginBottom={4}>
        <Image src={image} boxSize="250px" />
      </Flex>
      {canReplace && (
        <ImageDropzone
          name="image"
          initialImage={image}
          buttonWidth="100%"
          disabled={disabled}
          onChange={changeImage}
        />
      )}
    </Box>
  );
}
