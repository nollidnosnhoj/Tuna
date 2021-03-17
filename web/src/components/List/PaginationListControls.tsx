import { Button } from "@chakra-ui/button";
import { Flex, useColorModeValue } from "@chakra-ui/react";
import React, { PropsWithChildren } from "react";

type PaginationListControlProps = {
  page?: number;
  fetchPage?: (page: number) => void;
  totalPages?: number;
  hasNext?: boolean;
  hasPrevious?: boolean;
  isPreviousData?: boolean;
  isFetching?: boolean;
};

type PageButtonProps = {
  activePage?: boolean;
  disabled?: boolean;
  onClick?: () => void;
};

function PageButton(props: PropsWithChildren<PageButtonProps>) {
  const { onClick, activePage = false, disabled = false, children } = props;

  const activeStyle = {
    bg: useColorModeValue("primary.600", "primary.500"),
    color: useColorModeValue("white", "gray.200"),
  };

  return (
    <Button
      as="a"
      {...(activePage && activeStyle)}
      _hover={!disabled ? activeStyle : {}}
      cursor={disabled ? "not-allowed" : "pointer"}
      onClick={!disabled ? onClick : undefined}
    >
      {children}
    </Button>
  );
}

export default function PaginationListControls(
  props: PaginationListControlProps
) {
  const {
    page = 1,
    fetchPage: setPage,
    totalPages = 1,
    hasNext = false,
    hasPrevious = false,
    isFetching = false,
  } = props;

  const changePage = (newPage: number) => {
    const boundedPage = Math.max(0, Math.min(newPage, totalPages));
    setPage && setPage(boundedPage);
  };

  const goBackwards = (backInPages: number = 1) => {
    const newPage = page - backInPages;
    setPage && setPage(newPage);
  };

  const goForwards = (forwardInPages: number = 1) => {
    const newPage = page + forwardInPages;
    setPage && setPage(newPage);
  };

  return (
    <Flex>
      <PageButton
        disabled={!hasPrevious || isFetching}
        onClick={() => goBackwards()}
      >
        Previous
      </PageButton>
      <PageButton disabled={isFetching} onClick={() => changePage(1)}>
        1
      </PageButton>
      {page - 3 > 1 && <PageButton disabled={isFetching}>...</PageButton>}
      <PageButton disabled={isFetching} onClick={() => goBackwards(2)}>
        {page - 2}
      </PageButton>
      <PageButton disabled={isFetching} onClick={() => goBackwards(1)}>
        {page - 1}
      </PageButton>
      <PageButton disabled={isFetching} activePage>
        {page}
      </PageButton>
      <PageButton disabled={isFetching} onClick={() => goForwards(1)}>
        {page + 1}
      </PageButton>
      <PageButton disabled={isFetching} onClick={() => goForwards(2)}>
        {page + 2}
      </PageButton>
      {totalPages - (page + 2) > 1 && (
        <PageButton disabled={isFetching}>...</PageButton>
      )}
      <PageButton disabled={isFetching} onClick={() => changePage(totalPages)}>
        {totalPages}
      </PageButton>
      <PageButton
        disabled={!hasNext || isFetching}
        onClick={() => goForwards()}
      >
        Next
      </PageButton>
    </Flex>
  );
}
