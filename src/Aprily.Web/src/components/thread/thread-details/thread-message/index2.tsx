import type { Message } from "../../../../data/chat";
import "./index.css";

type Props = {
  message: Message;
};

export const ThreadMessage = ({ message }: Props) => {
  const isMe = message.from === "me";

  return (
    <div
      className={`d-flex mb-2 ${isMe ? "justify-content-end" : "justify-content-start"}`}
    >
      <div
        className={`thread-message rounded-top px-3 py-2 ${
          isMe
            ? "bg-primary text-white rounded-start thread-message-me"
            : "bg-body text-body rounded-end thread-message-other"
        }`}
      >
        {message.text}
      </div>
    </div>
  );
};
