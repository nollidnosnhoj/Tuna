import React, { useMemo, useState } from "react";
import { useFormik } from "formik";
import { Box, Flex } from "@chakra-ui/layout";
import Page from "~/components/Page";
import { Button, FormControl, Heading } from "@chakra-ui/react";
import TextInput from "~/components/Form/TextInput";
import TagInput from "~/components/Form/TagInput";
import AudioList from "~/features/audio/components/List";
import { useGetAudioListInfinite } from "~/features/audio/hooks/queries/useAudiosInfinite";
import InfiniteListControls from "~/components/List/InfiniteListControls";

export type AudioSearchValues = {
  q?: string;
  sort?: string;
  tags?: string[];
};

export default function AudioSearchPage(props: AudioSearchValues) {
  const [searchValues, setSearchValues] = useState<AudioSearchValues>(props);

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
  } = useGetAudioListInfinite("search/audios", queryParams);

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
          <Flex>
            {/* <HStack width="100%" spacing={4}>
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
            </HStack> */}
            <Flex width="100%" justifyContent="flex-end" alignItems="flex-end">
              <Button type="submit">Search</Button>
            </Flex>
          </Flex>
        </form>
      </Box>
      <AudioList audios={audios} />
      <InfiniteListControls
        fetchNext={fetchNextPage}
        hasNext={hasNextPage}
        isFetching={isFetchingNextPage}
      />
    </Page>
  );
}
