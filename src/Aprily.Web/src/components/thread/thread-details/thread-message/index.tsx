import { Box, Typography } from "@mui/material";
import type { Message } from "../../../../data/chat";

type Props = {
  message: Message;
};

export const ThreadMessage = ({ message }: Props) => {
  const isMe = message.from === "me";
  return (
    <Box
      sx={{
        display: "flex",
        justifyContent: isMe ? "flex-end" : "flex-start",
        mb: 1.5,
      }}
    >
      <Box
        sx={{
          maxWidth: "76%",
          px: 2,
          py: 1.25,
          borderRadius: 4,
          bgcolor: isMe ? "common.black" : "background.paper",
          color: isMe ? "common.white" : "common.black",

          ...(isMe
            ? {
                borderBottomRightRadius: 4,
              }
            : {
                borderBottomLeftRadius: 4,
              }),
        }}
      >
        <Typography variant="body1">{message.text}</Typography>
      </Box>
    </Box>
  );
};
