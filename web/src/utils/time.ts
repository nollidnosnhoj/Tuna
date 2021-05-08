import dayjs, { ConfigType } from "dayjs";
import duration from "dayjs/plugin/duration";
import utc from "dayjs/plugin/utc";
import relativeTime from "dayjs/plugin/relativeTime";

dayjs.extend(utc);
dayjs.extend(duration);
dayjs.extend(relativeTime);

export const relativeDate = (date?: ConfigType): string => {
  const todayUtc = dayjs.utc();
  const dateUtc = dayjs.utc(date);
  return dateUtc.from(todayUtc);
};
