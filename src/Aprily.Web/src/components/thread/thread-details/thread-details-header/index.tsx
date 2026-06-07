import { Avatar, IconButton, Stack, Typography } from "@mui/material";
import VideocamIcon from "@mui/icons-material/Videocam";
import LocalPhoneIcon from "@mui/icons-material/LocalPhone";
import ArrowBackIosNewIcon from "@mui/icons-material/ArrowBackIosNew";
import { useNavigate } from "@tanstack/react-router";
import type { Thread } from "../../../../data/chat";

type Props = {
  thread: Thread;
};

export const ThreadDetailsHeader = ({ thread }: Props) => {
  const navigate = useNavigate();

  return (
    <Stack
      direction={"row"}
      sx={{
        p: 2,
        justifyContent: "space-between",
        alignItems: "center",
        bgcolor: "background.paper",
      }}
    >
      <Stack direction={"row"} spacing={1} sx={{ alignItems: "center" }}>
        <IconButton onClick={() => navigate({ to: "/" })} color="inherit">
          <ArrowBackIosNewIcon />
        </IconButton>

        <Stack direction={"row"} spacing={1} sx={{ alignItems: "center" }}>
          <Avatar>{thread.title[0]}</Avatar>

          <Typography variant="h6" sx={{ fontWeight: 700 }}>
            {thread.title}
          </Typography>
        </Stack>
      </Stack>

      <Stack direction={"row"} sx={{ alignItems: "center" }}>
        <IconButton color="inherit">
          <VideocamIcon />
        </IconButton>

        <IconButton color="inherit">
          <LocalPhoneIcon />
        </IconButton>
      </Stack>
    </Stack>
  );
};
