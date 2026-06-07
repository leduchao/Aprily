import { Box, Stack } from "@mui/material";
import { type Thread } from "../../../data/chat";
import { useNavigate } from "@tanstack/react-router";
import { ThreadItem } from "./thread-item";

interface Props {
  threads: Thread[];
}

export const ThreadList = ({ threads }: Props) => {
  const navigate = useNavigate();

  const handleSelectThread = (threadId: string) => {
    navigate({ to: `/threads/${threadId}` });
  };

  return (
    <Box
      sx={{
        flex: 1,
        minHeight: 0,
        overflowY: "auto",
      }}
    >
      <Stack direction="column" sx={{ bgcolor: "background.paper" }}>
        {threads.map((thread, index) => {
          return (
            <ThreadItem
              key={index}
              thread={thread}
              onSelect={handleSelectThread}
            />
          );
        })}
      </Stack>
    </Box>
  );
};
