import type { ChatMessage } from "@/data/threads"
import { cn } from "@/lib/utils"

type MessageBubbleProps = {
  message: ChatMessage
}

export const MessageBubble = ({ message }: MessageBubbleProps) => {
  if (message.sender === "typing") {
    return (
      <div className="flex justify-start">
        <div className="rounded-2xl bg-muted px-4 py-2 text-lg leading-none">
          ...
        </div>
      </div>
    )
  }

  const isMine = message.sender === "me"

  return (
    <div className={cn("flex", isMine ? "justify-end" : "justify-start")}>
      <div
        className={cn(
          "max-w-[78%] rounded-2xl px-3 py-2 leading-relaxed",
          isMine
            ? "rounded-br-sm bg-primary text-gray-900"
            : "rounded-bl-sm bg-muted text-foreground"
        )}
      >
        {message.replyTo && (
          <div
            className={cn(
              "mb-2 border-l-2 pl-2 text-xs",
              isMine ? "border-primary-foreground/70" : "border-foreground/30"
            )}
          >
            <p className="font-semibold">{message.replyTo.name}</p>
            <p className="line-clamp-1 opacity-80">{message.replyTo.body}</p>
          </div>
        )}
        <p>{message.body}</p>
      </div>
    </div>
  )
}
