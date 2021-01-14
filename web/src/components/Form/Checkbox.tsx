import { Checkbox, FormControl, FormLabel, Switch } from "@chakra-ui/react";
import React from "react";
import { useFormContext } from "react-hook-form";

interface InputCheckboxProps {
  name: string;
  label?: string;
  disabled?: boolean;
  required?: boolean;
  toggleSwitch?: boolean;
}

const InputCheckbox: React.FC<InputCheckboxProps> = ({
  name,
  label,
  children,
  disabled = false,
  required = false,
  toggleSwitch = false,
}) => {
  const { register, errors } = useFormContext();
  return (
    <FormControl
      display="flex"
      alignItems="center"
      isInvalid={!!errors[name]}
      isRequired={required}
      paddingY={2}
    >
      {label && (
        <FormLabel htmlFor={name} mb="0">
          {label}
        </FormLabel>
      )}
      {toggleSwitch ? (
        <Switch id={name} name={name} disabled={disabled} ref={register} />
      ) : (
        <Checkbox id={name} name={name} ref={register} disabled={disabled}>
          {children}
        </Checkbox>
      )}
    </FormControl>
  );
};

export default InputCheckbox;
