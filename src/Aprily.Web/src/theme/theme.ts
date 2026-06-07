import { createTheme } from "@mui/material";
import { buttonOverrides } from "../theme/components/button";
import { palette } from "../theme/palete";
import { outlinedInputOverrides, textFieldOverrides } from "./components/input";
import { formControlOverrides } from "./components/form";
import { avatarOverrides } from "./components/avatar";

export const theme = createTheme({
  palette: palette,
  components: {
    MuiButton: buttonOverrides,
    MuiOutlinedInput: outlinedInputOverrides,
    MuiTextField: textFieldOverrides,
    MuiFormControl: formControlOverrides,
    MuiAvatar: avatarOverrides,
  },
});
