import {
  FormControl,
  FormLabel,
  Input,
  FormErrorMessage,
  Textarea,
} from "@chakra-ui/react";
import React from "react";

interface InputFieldProps {
  name: string;
  value: string;
  onChange: (e: React.ChangeEvent) => void;
  type?: string;
  error?: string;
  label?: string;
  required?: boolean;
  placeholder?: string;
  textArea?: boolean;
  disabled?: boolean;
}

const TextInput: React.FC<InputFieldProps> = ({
  name,
  value,
  onChange,
  label,
  placeholder,
  error,
  type = "text",
  required = false,
  textArea = false,
  disabled = false,
}) => {
  return (
    <FormControl
      id={name}
      isInvalid={!!error}
      isRequired={required}
      paddingY={2}
    >
      {label && <FormLabel htmlFor={name}>{label}</FormLabel>}
      {textArea ? (
        <Textarea
          name={name}
          value={value}
          onChange={onChange}
          placeholder={placeholder}
          disabled={disabled}
        />
      ) : (
        <Input
          type={type}
          name={name}
          value={value}
          onChange={onChange}
          placeholder={placeholder}
          disabled={disabled}
        />
      )}
      <FormErrorMessage>{error}</FormErrorMessage>
    </FormControl>
  );
};

export default TextInput;
