import {
  FormControl,
  FormErrorMessage,
  FormLabel,
  Input,
  Tag,
  TagCloseButton,
  TagLabel,
  Wrap,
  WrapItem,
} from "@chakra-ui/react";
import React, { useMemo, useState } from "react";
import { taggify } from "~/utils";

interface TagInputProps {
  name: string;
  value: string[];
  onChange?: (value: string[]) => void;
  onAdd: (value: string) => void;
  onRemove: (index: number) => void;
  error?: string | string[];
  disabled?: boolean;
  allowDuplicate?: boolean;
  maxLength?: number;
}

const TagInput: React.FC<TagInputProps> = ({
  name,
  value,
  onChange,
  onAdd,
  onRemove,
  error,
  disabled = false,
  allowDuplicate = false,
  maxLength = 10,
}) => {
  const [currentInput, setCurrentInput] = useState("");
  const [inputError, setInputError] = useState(error);
  const tags = useMemo(() => {
    return value.length === 0 ? [] : value.filter((val) => val.length > 0);
  }, [value]);

  const onInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setCurrentInput(e.target.value);
  };

  const onKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") {
      e?.preventDefault();
      onAddTag();
    }
  };

  const validateInput = (tag: string): boolean => {
    if (tag.length <= 0) {
      setInputError("A tag must have an character.");
      return false;
    }
    if (!allowDuplicate && value.includes(tag)) {
      setInputError("Tag aleady exists in set.");
      return false;
    }
    if (value.length >= maxLength) {
      setInputError("You reached the maximum amount of tags.");
      return false;
    }
    return true;
  };

  const onAddTag = () => {
    const taggifyTag = taggify(currentInput);
    if (!validateInput(taggifyTag)) return;
    // const newValues = [...value, taggifyTag];
    // onChange(newValues);
    onAdd(taggifyTag);
    setCurrentInput("");
    setInputError("");
  };

  const removeTag = (idx: number, e?: React.SyntheticEvent) => {
    if (idx < 0) return "";
    // const filtered = value.filter((_, i) => i !== idx);
    // onChange(filtered);
    onRemove(idx);
  };

  return (
    <FormControl paddingY={2} id={name} isInvalid={!!inputError}>
      <FormLabel>Tags</FormLabel>
      <Input
        name={name}
        value={currentInput}
        onChange={onInputChange}
        onKeyDown={onKeyDown}
        disabled={disabled}
      />
      <FormErrorMessage>{inputError}</FormErrorMessage>
      <Wrap marginTop={4}>
        {tags.map((tag, idx) => (
          <WrapItem key={idx}>
            <Tag size="md" borderRadius="full" colorScheme="primary">
              <TagLabel>{tag}</TagLabel>
              <TagCloseButton onClick={() => removeTag(idx)} />
            </Tag>
          </WrapItem>
        ))}
      </Wrap>
    </FormControl>
  );
};

export default TagInput;
