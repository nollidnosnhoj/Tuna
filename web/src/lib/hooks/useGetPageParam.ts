import { ParsedUrlQuery } from "querystring";
import { useMemo } from "react";

export function useGetPageParam(
  params: ParsedUrlQuery
): [number, ParsedUrlQuery] {
  const { page, ...queryParam } = params;

  const parsedPage = useMemo(() => {
    if (typeof page === "undefined") return 1;
    return parseInt(Array.isArray(page) ? page[0] : page);
  }, [page]);

  if (isNaN(parsedPage)) return [1, queryParam];

  return [parsedPage, queryParam];
}
