import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalCloseButton,
  ModalBody,
  Stack,
  ModalFooter,
  Button,
} from "@chakra-ui/react";
import { useRouter } from "next/router";
import React from "react";
import { useMemo } from "react";
import { useUser } from "~/features/user/hooks";
import SETTINGS from "~/lib/config";
import { toast } from "~/utils";
import { useResetPrivateKey } from "../../hooks";
import AudioShareItem from "./AudioShareItem";

interface AudioShareModalProps {
  isOpen: boolean;
  onClose: () => void;
  audioId: number;
  userId: string;
  isPrivate: boolean;
  privateKey?: string;
}

export default function AudioShareModal(props: AudioShareModalProps) {
  const { audioId, userId, isPrivate, privateKey, isOpen, onClose } = props;
  const router = useRouter();
  const { user } = useUser();
  const { mutateAsync: resetPrivateKeyAsync } = useResetPrivateKey(audioId);

  const onResetPrivateKey = async () => {
    const message =
      "NOTE: Resetting private key will make the current url invalid. Are you sure you want to do this?";

    if (confirm(message)) {
      const newKey = await resetPrivateKeyAsync();
      await router.push(`/audios/${audioId}?key=${newKey}`);
      toast("success", {
        description: "Private key reset.",
      });
    }
  };

  const audioUrl = useMemo(() => {
    const url = SETTINGS.FRONTEND_URL + `audios/${audioId}`;
    if (isPrivate && privateKey) {
      return url + `?key=${privateKey}`;
    }
    return url + "/";
  }, [audioId, isPrivate, privateKey]);

  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>Share</ModalHeader>
        <ModalCloseButton />
        <ModalBody marginY={4}>
          <Stack spacing={4} direction="column">
            <AudioShareItem label="URL" value={audioUrl} />
          </Stack>
        </ModalBody>
        {user?.id === userId && (
          <ModalFooter>
            <Button onClick={onResetPrivateKey}>Reset Private Key</Button>
          </ModalFooter>
        )}
      </ModalContent>
    </Modal>
  );
}
