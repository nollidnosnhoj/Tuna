import React from "react";
import _ from "lodash";
import { HStack } from "@chakra-ui/layout";
import { Button, IconButton } from "@chakra-ui/button";
import { ArrowLeftIcon, ArrowRightIcon } from "@chakra-ui/icons";

interface PaginationListControlsProps {
  currentPage: number;
  totalPages?: number;
  pageNeighbors?: number;
  onPageChange: (page: number) => void;
}

const LEFT_PAGE = "LEFT";
const RIGHT_PAGE = "RIGHT";

// https://www.digitalocean.com/community/tutorials/how-to-build-custom-pagination-with-react
export default function PaginationListControls(
  props: PaginationListControlsProps
) {
  const totalPages = Math.min(1, props.totalPages ?? 1);
  const currentPage = Math.max(1, Math.min(props.currentPage ?? 1, totalPages));
  // pageNeighbours can be: 0, 1 or 2
  const pageNeighbors = Math.max(0, Math.min(props.pageNeighbors ?? 0, 2));

  /**
   * Let's say we have 10 pages and we set pageNeighbours to 2
   * Given that the current page is 6
   * The pagination control will look like the following:
   *
   * (1) < {4 5} [6] {7 8} > (10)
   *
   * (x) => terminal pages: first and last page(always visible)
   * [x] => represents current page
   * {...x} => represents page neighbours
   */
  const fetchPageNumbers = () => {
    /**
     * totalNumbers: the total page numbers to show on the control
     * totalBlocks: totalNumbers + 2 to cover for the left(<) and right(>) controls
     */
    const totalNumbers = pageNeighbors * 2 + 3;
    const totalBlocks = totalNumbers + 2;

    if (totalPages > totalBlocks) {
      const startPage = Math.max(2, currentPage - pageNeighbors);
      const endPage = Math.min(totalPages - 1, currentPage + pageNeighbors);
      let pages: (number | typeof LEFT_PAGE | typeof RIGHT_PAGE)[] = _.range(
        startPage,
        endPage + 1
      );

      /**
       * hasLeftSpill: has hidden pages to the left
       * hasRightSpill: has hidden pages to the right
       * spillOffset: number of hidden pages either to the left or to the right
       */
      const hasLeftSpill = startPage > 2;
      const hasRightSpill = totalPages - endPage > 1;
      const spillOffset = totalNumbers - (pages.length + 1);

      switch (true) {
        // handle: (1) < {5 6} [7] {8 9} (10)
        case hasLeftSpill && !hasRightSpill: {
          const extraPages = _.range(startPage - spillOffset, startPage);
          pages = [LEFT_PAGE, ...extraPages, ...pages];
          break;
        }
        // handle: (1) {2 3} [4] {5 6} > (10)
        case !hasLeftSpill && hasRightSpill: {
          const extraPages = _.range(endPage + 1, endPage + spillOffset + 1);
          pages = [...pages, ...extraPages, RIGHT_PAGE];
          break;
        }
        // handle: (1) < {4 5} [6] {7 8} > (10)
        case hasLeftSpill && hasRightSpill:
        default: {
          pages = [LEFT_PAGE, ...pages, RIGHT_PAGE];
        }
      }

      return [1, ...pages, totalPages];
    }

    return _.range(1, totalPages + 1);
  };

  const goToPage = (page: number) => (evt: React.MouseEvent) => {
    evt.preventDefault();
    props.onPageChange(page);
  };

  const handleMoveLeft = (evt: React.MouseEvent) => {
    evt.preventDefault();
    goToPage(currentPage - pageNeighbors * 2 - 1);
  };

  const handleMoveRight = (evt: React.MouseEvent) => {
    evt.preventDefault();
    goToPage(currentPage + pageNeighbors * 2 + 1);
  };

  if (totalPages === 1) return null;

  const pages = fetchPageNumbers();

  return (
    <React.Fragment>
      <HStack>
        {pages.map((page, index) => {
          if (page === LEFT_PAGE)
            return (
              <IconButton
                key={index}
                icon={<ArrowLeftIcon />}
                aria-label="Previous"
                onClick={handleMoveLeft}
              />
            );
          if (page === RIGHT_PAGE)
            return (
              <IconButton
                key={index}
                icon={<ArrowRightIcon />}
                aria-label="Next"
                onClick={handleMoveRight}
              />
            );
          return (
            <Button
              key={index}
              isActive={currentPage === page}
              onClick={goToPage(page)}
            >
              {page}
            </Button>
          );
        })}
      </HStack>
    </React.Fragment>
  );
}
