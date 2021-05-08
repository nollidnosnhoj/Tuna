import { extendTheme } from "@chakra-ui/react";
import { createBreakpoints } from "@chakra-ui/theme-tools";

const breakpoints = createBreakpoints({
  sm: "320px",
  md: "768px",
  lg: "960px",
  xl: "1200px",
});

const theme = extendTheme({
  config: {
    useSystemColorMode: false,
    initialColorMode: "dark",
  },
  breakpoints,
  colors: {
    primary: {
      50: "#ffe7f8",
      100: "#f4bfe1",
      200: "#e897ca",
      300: "#de6eb3",
      400: "#d3469e",
      500: "#b92c84",
      600: "#912167",
      700: "#68174b",
      800: "#410b2d",
      900: "#1b0112",
    },
  },
  styles: {
    global: (props) => ({
      "html, body": {
        fontSize: "md",
        color: props.colorMode === "dark" ? "white" : "gray.800",
        lineHeight: "tall",
      },
      a: {
        color: props.colorMode === "dark" ? "primary.300" : "primary.500",
      },
    }),
  },
});

export default theme;
