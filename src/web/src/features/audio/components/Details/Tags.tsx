import {
  Flex,
  Wrap,
  WrapItem,
  Tag,
  TagLeftIcon,
  TagLabel,
} from "@chakra-ui/react";
import NextLink from "next/link";
import React from "react";
import { FaHashtag } from "react-icons/fa";

interface AudioTagsProps {
  tags: string[];
}

export default function AudioTags({ tags }: AudioTagsProps) {
  if (tags.length === 0) return null;

  return (
    <Flex alignItems="flex-end">
      <Wrap marginTop={2}>
        {tags.map((tag, idx) => (
          <WrapItem key={idx}>
            <NextLink href={`/tags/${tag}/`}>
              <Tag size="md" cursor="pointer">
                <TagLeftIcon as={FaHashtag} />
                <TagLabel>{tag}</TagLabel>
              </Tag>
            </NextLink>
          </WrapItem>
        ))}
      </Wrap>
    </Flex>
  );
}
