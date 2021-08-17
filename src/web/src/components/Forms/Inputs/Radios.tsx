import {
  FormControl,
  FormLabel,
  RadioGroup,
  Stack,
  Radio,
  FormErrorMessage,
  FormHelperText,
} from "@chakra-ui/react";
import React from "react";

type StringOrNumber = string | number;

type RadioInput = {
  name: string;
  value: StringOrNumber;
};

interface RadioInputsProps {
  name: string;
  value: StringOrNumber;
  onChange: (value: StringOrNumber) => void;
  options: RadioInput[];
  error?: string;
  isDisabled?: boolean;
  helperText?: string;
}

export default function RadioInputs({
  name,
  value,
  onChange,
  options,
  error,
  isDisabled,
  helperText,
}: RadioInputsProps) {
  return (
    <FormControl id={name} isInvalid={!!error} marginBottom={4}>
      <FormLabel>Privacy</FormLabel>
      <RadioGroup name={name} value={value} onChange={onChange}>
        <Stack spacing={2} direction="column">
          {options.map(({ name: n, value: v }, i) => (
            <Radio key={i} value={v} isDisabled={isDisabled}>
              {n}
            </Radio>
          ))}
        </Stack>
      </RadioGroup>
      <FormErrorMessage>{error}</FormErrorMessage>
      {helperText && <FormHelperText>{helperText}</FormHelperText>}
    </FormControl>
  );
}
