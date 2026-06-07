import type { Components, Theme } from "@mui/material";
import { teal } from "@mui/material/colors";

export const avatarOverrides: Components<Theme>["MuiAvatar"] = {
  styleOverrides: {
    root: {
      color: "white",
      backgroundColor: teal[700],
    },
  },
};
