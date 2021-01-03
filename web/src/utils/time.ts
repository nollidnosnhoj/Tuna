import dayjs, { ConfigType } from 'dayjs'
import utc from 'dayjs/plugin/utc'
import relativeTime from 'dayjs/plugin/relativeTime'

dayjs.extend(utc)
dayjs.extend(relativeTime)

export const formatDuration = (duration: number) => {
  if (!duration) return '0:00'
  const milliseconds = duration * 1000
  return duration > 3600
    ? dayjs.utc(milliseconds).format('H:mm:ss')
    : dayjs.utc(milliseconds).format('mm:ss');
}

export const relativeDate = (date?: ConfigType) => {
  const todayUtc = dayjs.utc()
  const dateUtc = dayjs.utc(date)
  return dateUtc.from(todayUtc)
}