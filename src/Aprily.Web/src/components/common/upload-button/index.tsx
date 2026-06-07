import { type ReactNode, useState } from "react";
import {
  Button,
  type ButtonProps,
  IconButton,
  type IconButtonProps,
  Stack,
  Typography,
} from "@mui/material";
import { styled } from "@mui/material/styles";
import UploadFileIcon from "@mui/icons-material/UploadFile";

const VisuallyHiddenInput = styled("input")({
  clip: "rect(0 0 0 0)",
  clipPath: "inset(50%)",
  height: 1,
  overflow: "hidden",
  position: "absolute",
  bottom: 0,
  left: 0,
  whiteSpace: "nowrap",
  width: 1,
});

type UploadButtonProps = {
  variant?: "button" | "icon";
  label?: string;
  icon?: ReactNode;
  multiple?: boolean;
  accept?: string;
  showFileNames?: boolean;
  onChange?: (files: File[]) => void;
  buttonProps?: ButtonProps;
  iconButtonProps?: IconButtonProps;
};

export const UploadButton = ({
  variant = "button",
  label = "Upload File",
  icon = <UploadFileIcon />,
  multiple = false,
  accept,
  showFileNames = true,
  onChange,
  buttonProps,
  iconButtonProps,
}: UploadButtonProps) => {
  const [fileNames, setFileNames] = useState<string[]>([]);

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(event.target.files || []);

    setFileNames(files.map((file) => file.name));
    onChange?.(files);
  };

  const input = (
    <VisuallyHiddenInput
      type="file"
      multiple={multiple}
      accept={accept}
      onChange={handleChange}
    />
  );

  return (
    <Stack spacing={1}>
      {variant === "icon" ? (
        <IconButton component="label" {...iconButtonProps}>
          {icon}
          {input}
        </IconButton>
      ) : (
        <Button
          component="label"
          variant="contained"
          startIcon={icon}
          {...buttonProps}
        >
          {label}
          {input}
        </Button>
      )}

      {showFileNames && fileNames.length > 0 && (
        <Stack spacing={0.5}>
          {fileNames.map((name) => (
            <Typography key={name} variant="body2">
              • {name}
            </Typography>
          ))}
        </Stack>
      )}
    </Stack>
  );
};
