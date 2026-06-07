import { type Thread } from "../../../data/chat";
import { useNavigate } from "@tanstack/react-router";
import { ThreadItem } from "./thread-item/index-v2";

interface Props {
  threads: Thread[];
}

export const ThreadList = ({ threads }: Props) => {
  const navigate = useNavigate();

  const handleSelectThread = (threadId: string) => {
    navigate({ to: `/threads/${threadId}` });
  };

  return (
    <div className="flex-grow-1 overflow-auto" style={{ minHeight: 0 }}>
      <div className="bg-body">
        {threads.map((thread) => (
          <ThreadItem
            key={thread.id}
            thread={thread}
            onSelect={handleSelectThread}
          />
        ))}
      </div>
    </div>
  );
};
