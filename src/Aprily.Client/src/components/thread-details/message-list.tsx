import { MessageBubble } from "@/components/thread-details/message-bubble"
import type { ChatMessage, MessageReactionType } from "@/lib/chat-api"
import { useEffect, useLayoutEffect, useRef, useState } from "react"

type MessageListProps = {
  messages: ChatMessage[]
  onReply: (message: ChatMessage) => void
  onReact: (message: ChatMessage, type: MessageReactionType) => void
}

export const MessageList = ({
  messages,
  onReply,
  onReact,
}: MessageListProps) => {
  const messageListRef = useRef<HTMLElement>(null)
  const highlightTimeoutRef = useRef<number | null>(null)
  const [highlightedMessageId, setHighlightedMessageId] = useState<
    string | null
  >(null)

  useLayoutEffect(() => {
    const messageList = messageListRef.current

    if (messageList) {
      messageList.scrollTop = messageList.scrollHeight
    }
  }, [messages.length])

  useEffect(() => {
    return () => {
      if (highlightTimeoutRef.current) {
        window.clearTimeout(highlightTimeoutRef.current)
      }
    }
  }, [])

  const navigateToMessage = (messageId: string) => {
    const messageElement = document.getElementById(`chat-message-${messageId}`)
    if (!messageElement || !messageListRef.current?.contains(messageElement)) {
      return
    }

    messageElement.scrollIntoView({ behavior: "smooth", block: "center" })
    setHighlightedMessageId(messageId)

    if (highlightTimeoutRef.current) {
      window.clearTimeout(highlightTimeoutRef.current)
    }

    highlightTimeoutRef.current = window.setTimeout(() => {
      setHighlightedMessageId(null)
      highlightTimeoutRef.current = null
    }, 1600)
  }

  return (
    <section
      ref={messageListRef}
      className="min-h-0 flex-1 scrollbar-none overflow-y-auto px-4 py-4"
    >
      <div className="flex min-h-full flex-col justify-end">
        <p className="mb-4 text-center text-sm text-muted-foreground">Today</p>

        <div className="space-y-3">
          {messages.map((message) => (
            <MessageBubble
              key={message.id}
              message={message}
              onReply={() => onReply(message)}
              onReact={(type) => onReact(message, type)}
              onNavigateToMessage={navigateToMessage}
              isHighlighted={highlightedMessageId === message.id}
            />
          ))}

          {messages.length > 0 && (
            <p className="pr-2 text-right text-xs text-muted-foreground">
              Delivered
            </p>
          )}
        </div>
      </div>
    </section>
  )
}
