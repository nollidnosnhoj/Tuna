import {
  FormControl,
  FormErrorMessage,
  FormLabel,
} from "@chakra-ui/form-control";
import { Input } from "@chakra-ui/input";
import { FormHelperText } from "@chakra-ui/react";
import { Textarea } from "@chakra-ui/textarea";
import React, { ChangeEvent } from "react";

interface TextInputProps {
  name: string;
  type?: string;
  onChange: (evt: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => void;
  onBlur: (evt: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => void;
  isTextArea?: boolean;
  error?: string;
  label?: string;
  size?: (string & Record<string, unknown>) | "lg" | "md" | "sm" | "xs";
  variant?:
    | (string & Record<string, unknown>)
    | "outline"
    | "filled"
    | "flushed"
    | "unstyled";
  focusBorderColor?: string;
  errorBorderColor?: string;
  isDisabled?: boolean;
  isRequired?: boolean;
  helperText?: string;
}

// eslint-disable-next-line react/display-name
const TextInput = React.forwardRef<any, TextInputProps>((props, ref) => {
  const {
    error,
    label,
    focusBorderColor,
    errorBorderColor,
    type = "text",
    isTextArea = false,
    size = "md",
    variant = "outline",
    isDisabled = false,
    isRequired = false,
    helperText,
    ...inputProps
  } = props;

  if (isTextArea) {
    return (
      <FormControl
        id={inputProps.name}
        isInvalid={!!error}
        isRequired={isRequired}
        paddingY={2}
      >
        {label && <FormLabel>{label}</FormLabel>}
        <Textarea
          {...inputProps}
          ref={ref}
          size={size}
          variant={variant}
          focusBorderColor={focusBorderColor}
          errorBorderColor={errorBorderColor}
          isDisabled={isDisabled}
          type={type}
        />
        <FormErrorMessage>{error}</FormErrorMessage>
        {helperText && <FormHelperText>{helperText}</FormHelperText>}
      </FormControl>
    );
  }

  return (
    <FormControl
      id={inputProps.name}
      isInvalid={!!error}
      isRequired={isRequired}
      paddingY={2}
    >
      {label && <FormLabel>{label}</FormLabel>}
      <Input
        {...inputProps}
        ref={ref}
        size={size}
        variant={variant}
        focusBorderColor={focusBorderColor}
        errorBorderColor={errorBorderColor}
        isDisabled={isDisabled}
        type={type}
      />
      <FormErrorMessage>{error}</FormErrorMessage>
      {helperText && <FormHelperText>{helperText}</FormHelperText>}
    </FormControl>
  );
});

export default TextInput;
