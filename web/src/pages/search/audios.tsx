import { useRouter } from "next/router";
import React, { useMemo, useState } from "react";
import queryString from "query-string";
import Page from "~/components/Page";
import { useFormik } from "formik";
import { Box, Flex, HStack } from "@chakra-ui/layout";
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
  Select,
} from "@chakra-ui/react";
import TextInput from "~/components/Form/TextInput";
import TagInput from "~/components/Form/TagInput";
import AudioList from "~/features/audio/components/List";
import { useAudiosInfinite } from "~/features/audio/hooks/queries";

type AudioSearchQuery = {
  q?: string;
  genre?: string;
  sort?: string;
  tags?: string[];
};

export default function AudioSearchPage() {
  const router = useRouter();
  const { query } = router;

  const [params, setParams] = useState<AudioSearchQuery>(() => ({
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

  const formik = useFormik<AudioSearchQuery>({
    initialValues: params,
    onSubmit: (values) => {
      setParams(values);
      const qs = queryString.stringify({
        ...values,
        tags: values.tags?.join(","),
      });
      router.replace(`/search/audios?${qs}`, undefined, {
        shallow: true,
      });
    },
  });

  const {
    handleChange,
    handleSubmit,
    values,
    errors,
    isSubmitting,
    setFieldValue,
  } = formik;

  const queryParams = useMemo(
    () => ({
      ...params,
      tags: params.tags?.join(","),
    }),
    [params]
  );

  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useAudiosInfinite("search/audios", queryParams);

  return (
    <Page removeSearchBar>
      <Box>
        <form onSubmit={handleSubmit}>
          <FormControl id="q">
            <TextInput
              name="q"
              value={values.q ?? ""}
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
                  value={values.tags ?? []}
                  onAdd={(tag) => {
                    setFieldValue("tags", [...(values.tags ?? []), tag]);
                  }}
                  onRemove={(index) => {
                    setFieldValue(
                      "tags",
                      values.tags?.filter((_, i) => i !== index)
                    );
                  }}
                  error={errors.tags}
                />
                <HStack spacing={4}>
                  <Box>
                    <GenreSelect
                      name="genre"
                      label="Genre"
                      value={values.genre ?? ""}
                      onChange={handleChange}
                      error={errors.genre}
                      placeholder="No Genre"
                    />
                  </Box>
                  <Box>
                    <FormControl id="sort">
                      <FormLabel>Sort</FormLabel>
                      <Select
                        name="sort"
                        value={values.sort}
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
        fetchNext={fetchNextPage}
        hasNext={hasNextPage}
        isFetching={isFetchingNextPage}
      />
    </Page>
  );
}
