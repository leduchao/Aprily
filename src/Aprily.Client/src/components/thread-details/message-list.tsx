import { MessageBubble } from "@/components/thread-details/message-bubble"
import type { ChatMessage } from "@/data/threads"
import { useLayoutEffect, useRef } from "react"

type MessageListProps = {
  messages: ChatMessage[]
}

export const MessageList = ({ messages }: MessageListProps) => {
  const messageListRef = useRef<HTMLElement>(null)

  useLayoutEffect(() => {
    const messageList = messageListRef.current

    if (messageList) {
      messageList.scrollTop = messageList.scrollHeight
    }
  }, [messages])

  return (
    <section
      ref={messageListRef}
      className="min-h-0 flex-1 scrollbar-none overflow-y-auto px-4 py-4"
    >
      <div className="flex min-h-full flex-col justify-end">
        <p className="mb-4 text-center text-sm text-muted-foreground">Today</p>

        <div className="space-y-3">
          {messages.map((message) => (
            <MessageBubble key={message.id} message={message} />
          ))}

          <p className="pr-2 text-right text-xs text-muted-foreground">
            Delivered
          </p>
        </div>
      </div>
    </section>
  )
}
