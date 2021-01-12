import {
  FormControl,
  FormLabel,
  Input,
  FormErrorMessage,
  Textarea,
} from "@chakra-ui/react";
import React from "react";
import { FieldError } from "react-hook-form";

interface InputFieldProps {
  name: string;
  type?: string;
  label?: string;
  isRequired?: boolean;
  error?: FieldError;
  placeholder?: string;
  isTextArea?: boolean;
  disabled?: boolean;
}

const TextInput = React.forwardRef<any, InputFieldProps>(
  (
    {
      name,
      type = "text",
      label,
      isRequired = false,
      error,
      placeholder,
      isTextArea = false,
      disabled = false,
    },
    ref
  ) => {
    return (
      <FormControl
        id={name}
        isInvalid={!!error}
        isRequired={isRequired}
        paddingY={2}
      >
        {label && <FormLabel htmlFor={name}>{label}</FormLabel>}
        {isTextArea ? (
          <Textarea
            ref={ref}
            name={name}
            placeholder={placeholder}
            disabled={disabled}
          />
        ) : (
          <Input
            type={type}
            ref={ref}
            name={name}
            placeholder={placeholder}
            disabled={disabled}
          />
        )}
        <FormErrorMessage>{error?.message}</FormErrorMessage>
      </FormControl>
    );
  }
);

export default TextInput;
