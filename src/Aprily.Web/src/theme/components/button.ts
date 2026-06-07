import type { Components, Theme } from "@mui/material";

export const buttonOverrides: Components<Theme>["MuiButton"] = {
  defaultProps: {
    disableElevation: true,
  },
  styleOverrides: {
    root: {
      borderRadius: 999,
    },
  },
};
