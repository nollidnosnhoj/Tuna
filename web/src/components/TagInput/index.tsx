import React, { useState, createRef } from "react";
import {
  Input,
  Tag,
  Flex,
  TagCloseButton,
  FormControl,
  FormLabel,
  FormErrorMessage,
} from "@chakra-ui/react";
import { FieldError } from "react-hook-form";

interface TagInputProps {
  name: string;
  value: string[];
  onChange: (val: string[]) => void;
  error?: FieldError;
  allowDuplicates?: boolean;
  maxLength?: number;
  disabled?: boolean;
}

const TagInput: React.FC<TagInputProps> = ({
  name,
  onChange,
  value,
  error: formError,
  allowDuplicates = false,
  maxLength = 10,
  disabled = false,
}) => {
  const [current, setCurrent] = useState("");
  const [error, setError] = useState(formError?.message ?? "");
  const divRef = createRef<HTMLDivElement>();
  const innerRef = createRef<HTMLInputElement>();

  const focusDiv = () => {
    if (divRef.current) {
      divRef.current.focus();
    }
  };

  const removeTag = (idx: number, e?: React.SyntheticEvent): string => {
    if (idx < 0) return "";
    e?.preventDefault();
    const lastTag = value[idx];
    const filtered = value.filter((x, i) => x !== lastTag);
    onChange(filtered);
    return lastTag;
  };

  const addCurrentTag = (e?: React.SyntheticEvent) => {
    e?.preventDefault();
    const newTags = [...value, current];
    onChange(newTags);
    focusDiv();
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (!e.isDefaultPrevented()) {
      setCurrent(e.target.value);
      setError("");
    }
  };

  const onKeyDown = (e: React.KeyboardEvent) => {
    switch (e.keyCode) {
      case 8:
        if (current.length === 0) {
          const lastTag = removeTag(value.length - 1, e);
          setCurrent(lastTag);
        }
        return;
      case 13:
      case 32:
      case 188:
        if (value.length === maxLength) {
          setError(`Cannot have more than ${maxLength} tags.`);
          return;
        }

        if (!allowDuplicates && value.includes(current)) {
          setError("Cannot have duplicate tags");
          return;
        }
        addCurrentTag(e);
        setCurrent("");
        return;
    }
  };

  return (
    <FormControl id="tags" isInvalid={!!error}>
      <FormLabel>Tags</FormLabel>
      <Flex
        ref={divRef}
        borderWidth="1px"
        borderRadius="0.375rem"
        py={2}
        px={4}
        rounded={2}
        flexWrap="wrap"
      >
        {value &&
          value.length > 0 &&
          value.map((val, index) => (
            <Tag key={index} marginRight={5} size="sm">
              {val}
              {disabled && <TagCloseButton onClick={(e) => removeTag(index)} />}
            </Tag>
          ))}
        <Input
          type="text"
          name={name}
          placeholder="Add tag..."
          _focus={{ outline: "none" }}
          _invalid={{ outline: "none" }}
          borderWidth={0}
          flex={1}
          minW="100px"
          value={current}
          onKeyDown={onKeyDown}
          onChange={handleChange}
          ref={innerRef}
          px={0}
          size="sm"
          disabled={disabled}
        />
      </Flex>
      <FormErrorMessage>{error}</FormErrorMessage>
    </FormControl>
  );
};

export default TagInput;
