import {
  Box,
  Checkbox,
  FormControl,
  FormLabel,
  HStack,
  Spacer,
  Switch,
} from "@chakra-ui/react";
import React from "react";

interface InputCheckboxProps {
  name: string;
  value: boolean;
  onChange: () => void;
  error?: string;
  label?: string;
  disabled?: boolean;
  required?: boolean;
  toggleSwitch?: boolean;
}

const InputCheckbox: React.FC<InputCheckboxProps> = ({
  name,
  value,
  onChange,
  error,
  label,
  children,
  disabled = false,
  required = false,
  toggleSwitch = false,
}) => {
  return (
    <FormControl
      display="flex"
      alignItems="center"
      isInvalid={!!error}
      isRequired={required}
      paddingY={2}
    >
      <HStack>
        <Box>
          {label && (
            <FormLabel htmlFor={name} mb="0">
              {label}
            </FormLabel>
          )}
        </Box>
        <Spacer />
        <Box>
          {toggleSwitch ? (
            <Switch
              id={name}
              name={name}
              disabled={disabled}
              defaultChecked={value}
              onChange={onChange}
            />
          ) : (
            <Checkbox
              id={name}
              name={name}
              disabled={disabled}
              checked={value}
              onChange={onChange}
            >
              {children}
            </Checkbox>
          )}
        </Box>
      </HStack>
    </FormControl>
  );
};

export default InputCheckbox;
