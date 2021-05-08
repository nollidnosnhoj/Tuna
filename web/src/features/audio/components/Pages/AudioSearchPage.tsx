import React, { useMemo, useState } from "react";
import { useFormik } from "formik";
import { Box, Flex } from "@chakra-ui/layout";
import Page from "~/components/Page";
import { Button, FormControl, Heading } from "@chakra-ui/react";
import TextInput from "~/components/form/TextInput";
import TagInput from "~/components/form/TagInput";
import AudioList from "~/features/audio/components/List";
import { useGetAudioListInfinite } from "~/features/audio/hooks/queries/useAudiosInfinite";
import InfiniteListControls from "~/components/InfiniteListControls";
import { useGetAudioListPagination } from "../../hooks/queries";
import PaginationListControls from "~/components/PaginationListControls";

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
    isFetching,
    page,
    setPage,
    totalPages,
  } = useGetAudioListPagination("search/audios", {
    params: {
      ...queryParams,
    },
    enabled: Boolean(searchValues.q),
  });

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
            onChange={(tags) => setFieldValue("tags", tags)}
            error={formErrors.tags}
          />
          <Flex>
            <Flex width="100%" justifyContent="flex-end" alignItems="flex-end">
              <Button type="submit">Search</Button>
            </Flex>
          </Flex>
        </form>
      </Box>
      <AudioList audios={audios} />
      {audios.length && (
        <PaginationListControls
          currentPage={page}
          onPageChange={setPage}
          pageNeighbors={2}
          totalPages={totalPages}
        />
      )}
    </Page>
  );
}
