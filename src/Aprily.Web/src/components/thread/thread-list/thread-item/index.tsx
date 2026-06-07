import { Avatar, Box, Chip, Stack, Typography } from "@mui/material";
import type { Thread } from "../../../../data/chat";

interface Props {
  thread: Thread;
  onSelect: (threadId: string) => void;
}

export const ThreadItem = ({ thread, onSelect }: Props) => {
  const handleSelectThread = (threadId: string) => {
    onSelect(threadId);
  };

  return (
    <Box
      onClick={() => handleSelectThread(thread.id)}
      sx={{
        display: "flex",
        alignItems: "center",
        gap: 2,
        p: 2,
        cursor: "pointer",

        "&:hover": {
          transition: "0.3s",
          bgcolor: "background.default",
        },
      }}
    >
      <Avatar>{thread.title[0]}</Avatar>

      <Box sx={{ flex: 1, minWidth: 0 }}>
        <Stack
          direction={"row"}
          sx={{ justifyContent: "space-between", alignItems: "center" }}
        >
          <Typography variant="body1" sx={{ fontWeight: 700 }}>
            {thread.title}
          </Typography>

          <Typography variant="caption">
            {thread.time || "Yesterday"}
          </Typography>
        </Stack>

        <Stack
          direction={"row"}
          sx={{ justifyContent: "space-between", alignItems: "center" }}
        >
          <Typography
            variant="body2"
            noWrap
            sx={
              thread.unread > 0
                ? { color: "text.primary" }
                : { color: "text.secondary" }
            }
          >
            {thread.lastMessage}
          </Typography>
          {thread.unread > 0 ? (
            <Chip
              size="small"
              label={thread.unread.toString()}
              color="primary"
              sx={{
                width: 20,
                height: 20,

                borderRadius: "50%",

                fontSize: 10,
                fontWeight: 700,

                "& .MuiChip-label": {
                  px: 0,
                },
              }}
            />
          ) : null}
        </Stack>
      </Box>
    </Box>
  );
};
