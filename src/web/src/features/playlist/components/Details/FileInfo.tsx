import { Table, Tbody, Tr, Td } from "@chakra-ui/react";
import React from "react";
import { formatDuration, formatFileSize } from "~/utils";

interface AudioFileInfoProps {
  duration: number;
  fileSize: number;
}

export default function AudioFileInfo({
  duration,
  fileSize,
}: AudioFileInfoProps) {
  return (
    <Table>
      <Tbody>
        <Tr>
          <Td>Duration</Td>
          <Td>{formatDuration(duration)}</Td>
        </Tr>
        <Tr>
          <Td>File Size</Td>
          <Td>{formatFileSize(fileSize)}</Td>
        </Tr>
      </Tbody>
    </Table>
  );
}
