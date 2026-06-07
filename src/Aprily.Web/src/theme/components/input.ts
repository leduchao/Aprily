import type { Components, Theme } from "@mui/material";

export const outlinedInputOverrides: Components<Theme>["MuiOutlinedInput"] = {
  defaultProps: {
    size: "small",
  },
  styleOverrides: {
    root: {
      borderRadius: 999,
      backgroundColor: "white",
    },
  },
};

export const textFieldOverrides: Components<Theme>["MuiTextField"] = {
  defaultProps: {
    size: "small",
  },
};
