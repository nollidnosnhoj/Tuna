import { Avatar, Box, Flex, Image } from "@chakra-ui/react";
import { PropsWithChildren, useState } from "react";
import PictureDropzone from "~/components/Shared/Picture/PictureDropzone";
import { Profile } from "~/lib/types/user";

interface PictureProps {
  name?: string;
  size?: string;
  user?: Profile;
  disabled?: boolean;
  canReplace?: boolean;
  onChange?: (file: File) => Promise<void>;
}

export default function UserPicture({
  user,
  size = "full",
  name = "image",
  disabled = false,
  canReplace = false,
  ...props
}: PropsWithChildren<PictureProps>) {
  const [image, setImage] = useState<string>(user.pictureUrl);

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
      <Flex justify="center">
        <Avatar name={user?.username} src={image} size={size} />
      </Flex>
      {canReplace && (
        <PictureDropzone
          name={name}
          image={image}
          disabled={disabled}
          onChange={changeImage}
        />
      )}
    </Box>
  );
}
