import { Checkbox, FormControl, FormLabel, Switch } from "@chakra-ui/react";
import React from "react";

interface InputCheckboxProps {
  name: string;
  label?: string;
  isInvalid?: boolean;
  disabled?: boolean;
  isRequired?: boolean;
  isSwitch?: boolean;
}

const InputCheckbox = React.forwardRef<any, InputCheckboxProps>(
  (
    {
      name,
      label,
      isInvalid = false,
      disabled = false,
      isRequired = false,
      isSwitch = false,
    },
    ref
  ) => {
    return (
      <FormControl
        display="flex"
        alignItems="center"
        isInvalid={isInvalid}
        isRequired={isRequired}
        paddingY={2}
      >
        {label && (
          <FormLabel htmlFor={name} mb="0">
            {label}
          </FormLabel>
        )}
        {isSwitch ? (
          <Switch id={name} name={name} disabled={disabled} ref={ref} />
        ) : (
          <Checkbox id={name} name={name} ref={ref} disabled={disabled} />
        )}
      </FormControl>
    );
  }
);

export default InputCheckbox;
