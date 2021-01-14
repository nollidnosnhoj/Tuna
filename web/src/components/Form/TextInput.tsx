import {
  FormControl,
  FormLabel,
  Input,
  FormErrorMessage,
  Textarea,
} from "@chakra-ui/react";
import React from "react";
import { FieldError, useFormContext } from "react-hook-form";
import { ErrorMessage } from "@hookform/error-message";

interface InputFieldProps {
  name: string;
  type?: string;
  label?: string;
  required?: boolean;
  placeholder?: string;
  textArea?: boolean;
  disabled?: boolean;
}

const TextInput: React.FC<InputFieldProps> = ({
  name,
  type = "text",
  label,
  placeholder,
  required = false,
  textArea = false,
  disabled = false,
}) => {
  const { register, errors } = useFormContext();

  return (
    <FormControl
      id={name}
      isInvalid={!!errors[name]}
      isRequired={required}
      paddingY={2}
    >
      {label && <FormLabel htmlFor={name}>{label}</FormLabel>}
      {textArea ? (
        <Textarea
          ref={register}
          name={name}
          placeholder={placeholder}
          disabled={disabled}
        />
      ) : (
        <Input
          type={type}
          ref={register}
          name={name}
          placeholder={placeholder}
          disabled={disabled}
        />
      )}
      <ErrorMessage name={name} errors={errors} as={FormErrorMessage} />
    </FormControl>
  );
};

export default TextInput;
