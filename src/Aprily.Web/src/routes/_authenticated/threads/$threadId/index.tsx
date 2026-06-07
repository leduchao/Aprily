import { createFileRoute } from "@tanstack/react-router";
import { Box, Button, Typography } from "@mui/material";
import { useNavigate } from "@tanstack/react-router";
import { useMemo, useState } from "react";
import {
  chatThreads,
  initialMessages,
  type Message,
} from "../../../../data/chat";
import { useQuery } from "@tanstack/react-query";
import api from "../../../../services/api";
import { ThreadMessage } from "../../../../components/thread/thread-details/thread-message";
import { ChatInputArea } from "../../../../components/thread/thread-details/chat-input-area";
import { ThreadDetailsHeader } from "../../../../components/thread/thread-details/thread-details-header";
import ThreadDetailsPage from "../../../../pages/thread-details";

export const Route = createFileRoute("/_authenticated/threads/$threadId/")({
  component: ThreadDetailsPage,
});

function RouteComponent() {
  const navigate = useNavigate();

  const { threadId } = Route.useParams();
  const thread = useMemo(
    () => chatThreads.find((item) => item.id === threadId),
    [threadId],
  );

  const [messageInput, setMessageInput] = useState("");
  const [messages, setMessages] = useState<Message[]>(
    threadId ? (initialMessages[threadId] ?? []) : [],
  );

  const handleSend = () => {
    const trimmed = messageInput.trim();
    if (!trimmed || !threadId) return;

    setMessages((current) => [...current, { from: "me", text: trimmed }]);
    setMessageInput("");
  };

  if (!thread) {
    return (
      <Box sx={{ minHeight: "100vh", padding: 4 }}>
        <Typography variant="h6">Thread not found</Typography>
        <Button variant="contained" onClick={() => navigate({ to: "/" })}>
          Back
        </Button>
      </Box>
    );
  }

  useQuery({
    queryKey: ["test"],
    queryFn: async () => api.get<string>("/chat/message"),
  });

  return (
    <Box
      sx={{
        minHeight: "100vh",
        display: "flex",
        flexDirection: "column",
        bgcolor: "grey.300",
      }}
    >
      <ThreadDetailsHeader thread={thread} />

      <Box
        sx={{
          flex: 1,
          overflowY: "auto",
          p: 2,
          backgroundColor: "grey.300",
        }}
      >
        {messages.map((message, index) => (
          <ThreadMessage key={index} message={message} />
        ))}
      </Box>

      <ChatInputArea
        value={messageInput}
        onChange={setMessageInput}
        onSend={handleSend}
        onUpload={(files) => console.log(files)}
      />
    </Box>
  );
}
