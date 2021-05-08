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
import React, { useCallback, useMemo, useState } from "react";
import * as yup from "yup";
import { taggify } from "~/utils";

interface TagInputProps {
  name: string;
  value: string[];
  onChange: (value: string[]) => void;
  validationSchema?: yup.SchemaOf<string>;
  formatTagCallback?: (rawTag: string) => string;
  placeholder?: string;
  error?: string;
  disabled?: boolean;
}

const TagInput: React.FC<TagInputProps> = ({
  name,
  value,
  onChange,
  validationSchema,
  formatTagCallback,
  error,
  disabled = false,
}) => {
  const [currentInput, setCurrentInput] = useState("");
  const [inputError, setInputError] = useState(error);
  const tags = useMemo(() => {
    return value.length === 0 ? [] : value.filter((val) => val.length > 0);
  }, [value]);

  const applyValidationSchema = useCallback(
    (input: string) => {
      return new Promise<[boolean, string]>((resolve) => {
        validationSchema
          ?.validate(input)
          .then(() => resolve([true, ""]))
          .catch((err) => {
            let message: string = "Unknown validation error.";
            if (err instanceof yup.ValidationError) {
              message = Array.isArray(err.errors) ? err.errors[0] : err.errors;
            }
            resolve([false, message]);
          });
      });
    },
    [validationSchema]
  );

  const onInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setCurrentInput(e.target.value);
  };

  const onKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") {
      e?.preventDefault();
      onAddTag();
    }
  };

  const onAddTag = async () => {
    const taggifyTag = formatTagCallback?.(currentInput) ?? currentInput;
    if (validationSchema) {
      var [isValid, errorMessage] = await applyValidationSchema(taggifyTag);
      if (!isValid) {
        setInputError(errorMessage);
        return;
      }
    }
    onChange([...tags, taggifyTag]);
    setCurrentInput("");
    setInputError("");
  };

  const removeTag = (idx: number, e?: React.SyntheticEvent) => {
    if (idx < 0 || idx >= tags.length) return;
    const filtered = [...tags];
    filtered.splice(idx, 1);
    onChange(filtered);
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
