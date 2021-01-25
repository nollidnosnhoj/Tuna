import { Box, Flex, Image } from "@chakra-ui/react";
import { PropsWithChildren, useState } from "react";
import PictureDropzone from "~/components/Shared/Picture/PictureDropzone";
import { AudioDetail, AudioListItem } from "~/lib/types/audio";

interface AudioPictureProps {
  name: string;
  audio?: AudioDetail;
  disabled?: boolean;
  canReplace?: boolean;
  onChange?: (file: File) => Promise<void>;
}

export default function AudioPicture({
  audio,
  name = "image",
  disabled = false,
  canReplace = false,
  ...props
}: PropsWithChildren<AudioPictureProps>) {
  const [image, setImage] = useState<string>(audio?.pictureUrl);

  /** When image changes (after submitting crop) */
  const changeImage = async (file: File) => {
    if (props.onChange) {
      props
        .onChange(file)
        .then(() => setImage(window.URL.createObjectURL(file)));
    } else {
      setImage(window.URL.createObjectURL(file));
    }
  };

  return (
    <Box>
      <Flex justify="center" align="center">
        <Box textAlign="center">
          <Image src={image} boxSize="250px" marginBottom={4} />
          {canReplace && (
            <PictureDropzone
              name="image"
              image={image}
              disabled={disabled}
              onChange={changeImage}
            />
          )}
        </Box>
      </Flex>
    </Box>
  );
}
