import { Button } from "@chakra-ui/react";
import React from "react";

type InfiniteListControlsProps = {
  hasNext?: boolean;
  isFetching?: boolean;
  fetchNext?: () => void;
};

const InfiniteListControls = (props: InfiniteListControlsProps) => {
  const { hasNext = false, isFetching = false, fetchNext } = props;

  if (!hasNext) return null;

  return (
    <Button
      width="100%"
      variant="outline"
      disabled={isFetching}
      onClick={fetchNext}
    >
      {isFetching ? "Loading..." : "Load more"}
    </Button>
  );
};

export default InfiniteListControls;
