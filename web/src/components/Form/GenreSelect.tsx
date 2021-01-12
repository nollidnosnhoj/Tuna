import { FormControl, FormLabel, Select } from "@chakra-ui/react";
import React, { useEffect, useState } from "react";
import { GenreDto } from "~/lib/types/genre";
import fetch from "~/lib/fetcher";

interface GenreSelectProps {
  name: string;
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
  isRequired?: boolean;
  isDisabled?: boolean;
}

const GenreSelect: React.FC<GenreSelectProps> = ({
  name,
  value,
  onChange,
  placeholder,
  isRequired,
  isDisabled,
}) => {
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
    <FormControl paddingY={2} id={name} isRequired={isRequired}>
      <FormLabel>Genre</FormLabel>
      <Select
        name={name}
        value={value}
        onChange={(e) => onChange(e.target.value)}
        placeholder={placeholder}
        isDisabled={isDisabled}
      >
        {genres.map((g, i) => (
          <option key={i} value={g.slug}>
            {g.name}
          </option>
        ))}
      </Select>
    </FormControl>
  );
};

export default GenreSelect;
