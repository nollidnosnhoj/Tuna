import {
  FormControl,
  FormErrorMessage,
  FormLabel,
} from "@chakra-ui/form-control";
import { Input, InputProps } from "@chakra-ui/input";
import { FormHelperText } from "@chakra-ui/react";
import React from "react";

type ITextInputProps = InputProps & {
  error?: string;
  label?: string;
  helperText?: string;
};

// eslint-disable-next-line react/display-name
const InputField = React.forwardRef(
  (props: ITextInputProps, ref: React.ForwardedRef<HTMLInputElement>) => {
    const { error, label, helperText, ...inputProps } = props;

    const { name, isRequired } = inputProps;

    return (
      <FormControl
        id={name}
        isInvalid={!!error}
        isRequired={isRequired}
        paddingY={2}
      >
        {label && <FormLabel>{label}</FormLabel>}
        <Input {...inputProps} ref={ref} />
        <FormErrorMessage>{error}</FormErrorMessage>
        {helperText && <FormHelperText>{helperText}</FormHelperText>}
      </FormControl>
    );
  }
);

export default InputField;
