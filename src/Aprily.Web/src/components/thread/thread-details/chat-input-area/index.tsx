import { IconButton, Stack, TextField } from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import SendIcon from "@mui/icons-material/Send";
import { UploadButton } from "../../../common/upload-button";

type Props = {
  value: string;
  onChange: (value: string) => void;
  onSend: () => void;
  onUpload?: (files: File[]) => void;
};

export const ChatInputArea = ({ value, onChange, onSend, onUpload }: Props) => {
  const disabled = !value.trim();

  return (
    <Stack
      direction="row"
      spacing={1}
      sx={{
        py: 2,
        px: 1,
        // bgcolor: "inherit",
      }}
    >
      <UploadButton
        variant="icon"
        icon={<AddIcon />}
        multiple
        showFileNames={false}
        onChange={onUpload}
        iconButtonProps={{
          sx: {
            bgcolor: "background.paper",
            color: "text.primary",

            "&:hover": {
              bgcolor: "common.black",
              color: "common.white",
            },
          },
        }}
      />

      <TextField
        fullWidth
        size="small"
        placeholder="Enter message..."
        value={value}
        onChange={(event) => onChange(event.target.value)}
        onKeyDown={(event) => {
          if (event.key === "Enter" && !event.shiftKey && !disabled) {
            event.preventDefault();
            onSend();
          }
        }}
        sx={{
          bgcolor: "common.white",
          borderRadius: 999,

          "& fieldset": {
            border: "none",
          },
        }}
      />

      <IconButton
        sx={{ bgcolor: "common.black", color: "common.white" }}
        onClick={onSend}
        disabled={disabled}
      >
        <SendIcon />
      </IconButton>
    </Stack>
  );
};
