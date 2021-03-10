// https://stackoverflow.com/a/2686098
export function formatNumberAbbrv(num: number) {
  // dont format if the number is in the one-thosands
  if (num < 10000) return addCommasToNumber(num);

  let result = "";
  
  const labels = ['K', 'M', 'B', 'T'];

  for (let i = labels.length - 1; i >= 0; i--) {
      let size = Math.pow(10, (i + 1) * 3);
      if (size <= num) {
          num = Math.round(num * 10 / size) / 10;
          if ((num === 1000) && (i < labels.length - 1)) {
              return ""+num;
          }
          if (num > 100) num = Math.floor(num);

          return num + labels[i];
      }
  }

  return result + num;
}

export function addCommasToNumber(num: number): string {
  // convert number into string
  let numString = num+"";

  if (!numString) return numString;

  // split the integers and decimals
  const [integers, decimals]: Array<string | undefined> = numString.split('.');

  if (!integers) return numString;

  let result = '';

  // loop through each digits from 10^0 to 10^n
  for (let i = integers.length - 1; i >= 0; i--) {
    // every 10 to the third power, add comma
    // edge case for 999 or 999999... dont add comma at the beginning
    if (((integers.length - i) % 3 === 0) && i !== 0) {
      result = ',' + integers[i] + result;
    } else {
      result = integers[i] + result;
    }
  }

  // if there's no decimals, return integers
  if (!decimals) return result;

  // return with decimals
  return result + '.' + decimals;
}

export const formatFileSize = (size: number) => {
  if (size === 0) return '0 Bytes'
  const units = ['Bytes', 'kB', 'MB', 'GB'];

  // get exponential
  const e = Math.min(Math.floor(Math.log(size) / Math.log(1024)), units.length - 1)

  // conver the bytes into the exponential unit byte
  size = Number((size / Math.pow(1024, e)).toPrecision(3));
  return size.toLocaleString() + ' ' + units[e];
}