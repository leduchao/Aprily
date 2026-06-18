import type { ChatMessage } from "@/lib/chat-api"
import { cn } from "@/lib/utils"

type MessageBubbleProps = {
  message: ChatMessage
}

export const MessageBubble = ({ message }: MessageBubbleProps) => {
  return (
    <div
      className={cn("flex", message.isMine ? "justify-end" : "justify-start")}
    >
      <div
        className={cn(
          "max-w-[78%] rounded-2xl px-3 py-2 leading-relaxed",
          message.isMine
            ? "rounded-br-sm bg-primary text-gray-900"
            : "rounded-bl-sm bg-muted text-foreground"
        )}
      >
        <p>{message.content}</p>
      </div>
    </div>
  )
}
