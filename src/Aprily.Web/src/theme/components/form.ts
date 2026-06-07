import type { Components, Theme } from "@mui/material";

export const formControlOverrides: Components<Theme>["MuiFormControl"] = {
  defaultProps: {
    size: "small",
  },
};
