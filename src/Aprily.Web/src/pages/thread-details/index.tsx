import { useParams, useNavigate } from "@tanstack/react-router";
import { useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";

import { chatThreads, initialMessages, type Message } from "../../data/chat";
import api from "../../services/api";
import { ThreadDetailsHeader } from "../../components/thread/thread-details/thread-details-header/index2";
import { ThreadMessage } from "../../components/thread/thread-details/thread-message/index2";
import { ChatInputArea } from "../../components/thread/thread-details/chat-input-area/index2";

export default function ThreadDetailsPage() {
  const navigate = useNavigate();

  const { threadId } = useParams({
    from: "/_authenticated/threads/$threadId/",
  });

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

  useQuery({
    queryKey: ["test"],
    queryFn: async () => api.get<string>("/chat/message"),
  });

  if (!thread) {
    return (
      <div className="min-vh-100 p-4">
        <h5>Thread not found</h5>

        <button
          type="button"
          className="btn btn-primary"
          onClick={() => navigate({ to: "/" })}
        >
          Back
        </button>
      </div>
    );
  }

  return (
    <div className="min-vh-100 d-flex flex-column bg-secondary-subtle">
      <ThreadDetailsHeader thread={thread} />

      <main className="flex-grow-1 overflow-auto p-3 bg-secondary-subtle">
        {messages.map((message, index) => (
          <ThreadMessage key={index} message={message} />
        ))}
      </main>

      <ChatInputArea
        value={messageInput}
        onChange={setMessageInput}
        onSend={handleSend}
        onUpload={(files) => console.log(files)}
      />
    </div>
  );
}
