import {
  FormControl,
  FormErrorMessage,
  FormLabel,
  Select,
} from "@chakra-ui/react";
import React, { useEffect, useState } from "react";
import { useFormContext } from "react-hook-form";
import { ErrorMessage } from "@hookform/error-message";
import { GenreDto } from "~/lib/types/genre";
import fetch from "~/lib/fetcher";

interface GenreSelectProps {
  name: string;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
}

const GenreSelect: React.FC<GenreSelectProps> = ({
  name,
  placeholder,
  required = false,
  disabled = false,
}) => {
  const { register, errors } = useFormContext();
  const [genres, setGenres] = useState<GenreDto[]>([]);

  useEffect(() => {
    async function getGenres() {
      try {
        const data = await fetch<GenreDto[]>("genres");
        setGenres(data);
      } catch (err) {
        setGenres([]);
      }
    }
    getGenres();
  }, []);

  return (
    <FormControl
      paddingY={2}
      id={name}
      isRequired={required}
      isInvalid={!!errors[name]}
    >
      <FormLabel>Genre</FormLabel>
      <Select
        name={name}
        ref={register}
        placeholder={placeholder}
        isDisabled={disabled}
      >
        {genres.map((g, i) => (
          <option key={i} value={g.slug}>
            {g.name}
          </option>
        ))}
      </Select>
      <ErrorMessage name={name} errors={errors} as={FormErrorMessage} />
    </FormControl>
  );
};

export default GenreSelect;
