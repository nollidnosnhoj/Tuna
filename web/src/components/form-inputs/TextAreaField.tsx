// eslint-disable-next-line react/display-name
import React from "react";
import {
  FormControl,
  FormErrorMessage,
  FormLabel,
} from "@chakra-ui/form-control";
import { FormHelperText, Textarea, TextareaProps } from "@chakra-ui/react";

type ITextAreaProps = TextareaProps & {
  error?: string;
  label?: string;
  helperText?: string;
};

// eslint-disable-next-line react/display-name
const TextAreaField = React.forwardRef(
  (props: ITextAreaProps, ref: React.ForwardedRef<HTMLTextAreaElement>) => {
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
        <Textarea {...inputProps} ref={ref} />
        <FormErrorMessage>{error}</FormErrorMessage>
        {helperText && <FormHelperText>{helperText}</FormHelperText>}
      </FormControl>
    );
  }
);

export default TextAreaField;
