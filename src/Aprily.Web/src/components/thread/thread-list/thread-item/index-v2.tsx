import "./index.css";
import type { Thread } from "../../../../data/chat";
import { Avatar } from "../../../common/avatar";

interface Props {
  thread: Thread;
  onSelect: (threadId: string) => void;
}

export const ThreadItem = ({ thread, onSelect }: Props) => {
  const handleSelectThread = () => {
    onSelect(thread.id);
  };

  return (
    <button
      type="button"
      className="thread-item w-100 border-0 bg-transparent d-flex align-items-center gap-3 p-3 text-start"
      onClick={handleSelectThread}
    >
      <Avatar name={thread.title} size={40} />

      <div className="flex-grow-1 min-w-0">
        <div className="d-flex justify-content-between align-items-center gap-2">
          <div className="fw-bold text-truncate">{thread.title}</div>

          <small className="text-muted flex-shrink-0">
            {thread.time || "Yesterday"}
          </small>
        </div>

        <div className="d-flex justify-content-between align-items-center gap-2">
          <div
            className={`small text-truncate ${
              thread.unread > 0 ? "text-body" : "text-muted"
            }`}
          >
            {thread.lastMessage}
          </div>

          {thread.unread > 0 && (
            <span
              className="badge rounded-circle bg-primary d-inline-flex align-items-center justify-content-center flex-shrink-0"
              style={{
                width: 20,
                height: 20,
                fontSize: 10,
                fontWeight: 700,
                padding: 0,
              }}
            >
              {thread.unread}
            </span>
          )}
        </div>
      </div>
    </button>
  );
};
