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
      ".rhap_container": {
        fontFamily: "system-ui, sans-serif",
        backgroundColor: props.colorMode === "dark" ? "gray.800" : "white",
        boxShadow: "none",
        padding: 0
      },
      ".rhap_controls-section": {
        alignItems: 'normal',
        marginX: 4,
        marginY: 2
      },
      ".rhap_progress-container": {
        marginX: 0,
        height: "10px",
        alignItems: 'flex-start'
      },
      ".rhap_progress-bar": {
        borderRadius: 0,
        height: "none"
      },
      ".rhap_progress-filled": {
        backgroundColor: "primary.400"
      },
      ".rhap_time": {
        display: 'flex',
        alignItems: 'center',
        marginX: 2
      },
      ".rhap_nowPlaying": {
        a: {
          color: props.colorMode === 'dark' ? "gray.100" : "gray.700",
        },
        'a:hover': {
          textDecoration: 'underline'
        }
      },
      ".rhap_progress-indicator, .rhap_volume-indicator, .rhap_volume-filled": {
        backgroundColor: props.colorMode === 'dark' ? "gray.100" : "gray.700"
      },
      ".rhap_time, .rhap_repeat-button, .rhap_main-controls-button, .rhap_volume-button": {
        color: props.colorMode === 'dark' ? "gray.100" : "gray.700"
      },
      ".rhap_main-controls-button:hover": {
        color: props.colorMode  === 'dark' ? 'gray.300' : "gray.600"
      }
    }),
  },
});

export default theme;
