import { useRouter } from "next/router";
import React, { useMemo, useState } from "react";
import { useFormik } from "formik";
import { Box, Flex, HStack } from "@chakra-ui/layout";
import Page from "~/components/Page";
import GenreSelect from "~/components/Form/GenreSelect";
import {
  Accordion,
  AccordionButton,
  AccordionIcon,
  AccordionItem,
  AccordionPanel,
  Button,
  FormControl,
  FormLabel,
  Heading,
  Select,
} from "@chakra-ui/react";
import TextInput from "~/components/Form/TextInput";
import TagInput from "~/components/Form/TagInput";
import AudioList from "~/features/audio/components/List";
import { useAudiosInfinite } from "~/features/audio/hooks/queries";

type AudioSearchValues = {
  q?: string;
  genre?: string;
  sort?: string;
  tags?: string[];
};

export default function AudioSearchPage() {
  const router = useRouter();
  const { query } = router;

  const [searchValues, setSearchValues] = useState<AudioSearchValues>(() => ({
    q: (Array.isArray(query["q"]) ? query["q"][0] : query["q"]) || "",
    genre:
      (Array.isArray(query["sort"]) ? query["sort"][0] : query["sort"]) || "",
    sort:
      (Array.isArray(query["sort"]) ? query["sort"][0] : query["sort"]) || "",
    tags: Array.isArray(query["tags"])
      ? query["tags"]
      : query["tags"]
      ? query["tags"].split(",").map((t) => t.trim())
      : [],
  }));

  const formik = useFormik<AudioSearchValues>({
    initialValues: searchValues,
    onSubmit: (values) => {
      setSearchValues(values);
    },
  });

  const {
    handleChange,
    handleSubmit,
    values: formValues,
    errors: formErrors,
    setFieldValue,
  } = formik;

  const queryParams = useMemo(
    () => ({
      ...searchValues,
      tags: searchValues.tags?.join(","),
    }),
    [searchValues]
  );

  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useAudiosInfinite("search/audios", queryParams);

  return (
    <Page title="Search audios | Audiochan" removeSearchBar>
      <Heading>
        Search {searchValues.q ? `results for ${searchValues.q}` : ""}
      </Heading>
      <Box>
        <form onSubmit={handleSubmit}>
          <FormControl id="q">
            <TextInput
              name="q"
              value={formValues.q ?? ""}
              onChange={handleChange}
              placeholder="Search..."
              size="lg"
            />
          </FormControl>
          <Accordion allowToggle>
            <AccordionItem>
              <h2>
                <AccordionButton>
                  <Box flex="1" textAlign="left">
                    Filter
                  </Box>
                  <AccordionIcon />
                </AccordionButton>
              </h2>
              <AccordionPanel>
                <TagInput
                  name="tags"
                  value={formValues.tags ?? []}
                  onAdd={(tag) => {
                    setFieldValue("tags", [...(formValues.tags ?? []), tag]);
                  }}
                  onRemove={(index) => {
                    setFieldValue(
                      "tags",
                      formValues.tags?.filter((_, i) => i !== index)
                    );
                  }}
                  error={formErrors.tags}
                />
                <HStack spacing={4}>
                  <Box>
                    <GenreSelect
                      name="genre"
                      label="Genre"
                      value={formValues.genre ?? ""}
                      onChange={handleChange}
                      error={formErrors.genre}
                      placeholder="No Genre"
                    />
                  </Box>
                  <Box>
                    <FormControl id="sort">
                      <FormLabel>Sort</FormLabel>
                      <Select
                        name="sort"
                        value={formValues.sort}
                        onChange={handleChange}
                      >
                        <option value="latest">Latest</option>
                      </Select>
                    </FormControl>
                  </Box>
                </HStack>
              </AccordionPanel>
            </AccordionItem>
          </Accordion>
          <Flex marginTop={4} justifyContent="flex-end">
            <Button type="submit">Submit</Button>
          </Flex>
        </form>
      </Box>
      <AudioList
        type="infinite"
        audios={audios}
        fetchPage={fetchNextPage}
        hasNext={hasNextPage}
        isFetching={isFetchingNextPage}
      />
    </Page>
  );
}
