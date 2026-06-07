import {
  FormControl,
  FormHelperText,
  IconButton,
  InputAdornment,
  InputLabel,
  OutlinedInput,
  type OutlinedInputProps,
} from "@mui/material";
import { useId, useState } from "react";

import VisibilityIcon from "@mui/icons-material/Visibility";
import VisibilityOffIcon from "@mui/icons-material/VisibilityOff";
import { useTranslation } from "react-i18next";

interface Props extends OutlinedInputProps {
  label?: string;
  helperText?: string;
}

export const PasswordInput = ({
  label,
  helperText,
  error,
  id,
  ...props
}: Props) => {
  const { t } = useTranslation();

  const generatedId = useId();
  const inputId = id || `${generatedId}-input`;

  const finalLabel = label ?? t("common.password");

  const [showPassword, setShowPassword] = useState(false);
  const handleClickShowPassword = () => setShowPassword((show) => !show);

  return (
    <FormControl variant="outlined" error={error} fullWidth>
      <InputLabel htmlFor={inputId}>{finalLabel}</InputLabel>

      <OutlinedInput
        {...props}
        id={inputId}
        type={showPassword ? "text" : "password"}
        label={label}
        endAdornment={
          <InputAdornment position="end">
            <IconButton
              aria-label={
                showPassword ? "hide the password" : "display the password"
              }
              onClick={handleClickShowPassword}
              edge="end"
            >
              {showPassword ? <VisibilityOffIcon /> : <VisibilityIcon />}
            </IconButton>
          </InputAdornment>
        }
      />

      {helperText && <FormHelperText>{helperText}</FormHelperText>}
    </FormControl>
  );
};
