import { Box, Heading, Text } from "@chakra-ui/layout";
import { GetServerSideProps } from "next";
import Router from "next/router";
import React, { useMemo, useState } from "react";
import { useQuery } from "react-query";
import Page from "~/components/Page";
import AudioList from "~/features/audio/components/List";
import { fetch } from "~/utils/api";
import { PagedList } from "~/lib/types";
import { Audio } from "~/features/audio/types";
import { Profile } from "~/features/user/types";
import { useFormik } from "formik";
import { Button, Flex, FormControl } from "@chakra-ui/react";
import TextInput from "~/components/Form/TextInput";
import InfiniteListControls from "~/components/List/InfiniteListControls";

type SearchValues = { q: string };

export const getServerSideProps: GetServerSideProps<SearchValues> = async ({
  query,
}) => {
  let searchTerm: string = "";

  if (query["q"]) {
    searchTerm = Array.isArray(query["q"]) ? query["q"][0] : query["q"];
  }

  return {
    props: {
      q: searchTerm,
    },
  };
};

type SearchResponse = {
  audios: PagedList<Audio>;
  users: PagedList<Profile>;
};

export default function SearchPage(props: SearchValues) {
  const [searchValues, setSearchValues] = useState<SearchValues>(props);

  const formik = useFormik<SearchValues>({
    initialValues: searchValues,
    onSubmit: (values) => {
      if (!values.q) return;
      setSearchValues(values);
    },
  });

  const { handleChange, handleSubmit, values: formValues } = formik;

  const { data, isFetching } = useQuery<SearchResponse>(
    ["search", searchValues],
    () => fetch<SearchResponse>("search", searchValues),
    {
      enabled: !!searchValues.q,
    }
  );

  const [audios, audiosCount] = useMemo(() => {
    return [data?.audios.items ?? [], data?.audios.count ?? 0];
  }, [data]);

  const [users, usersCount] = useMemo(() => {
    return [data?.users.items ?? [], data?.users.count ?? 0];
  }, [data]);

  const noResultsFound = useMemo(() => {
    return !audiosCount && !usersCount;
  }, [audiosCount, usersCount]);

  return (
    <Page title={`Search everything | Audiochan`} removeSearchBar>
      <Box>
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
            <Flex marginTop={4} justifyContent="flex-end">
              <Button type="submit">Submit</Button>
            </Flex>
          </form>
        </Box>
        {!searchValues.q && (
          <Box>
            <Text>Search for anything here.</Text>
          </Box>
        )}
        {data && (
          <React.Fragment>
            {noResultsFound && <Text>hahaha</Text>}
            {audios.length > 0 && (
              <Box>
                <Heading as="h2" size="lg">
                  Audios
                </Heading>
                <AudioList audios={audios} />
              </Box>
            )}
          </React.Fragment>
        )}
      </Box>
    </Page>
  );
}
