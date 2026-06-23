import { MessageBubble } from "@/components/thread-details/message-bubble"
import type { ChatMessage, MessageReactionType } from "@/lib/chat-api"
import { useEffect, useLayoutEffect, useRef, useState } from "react"

type MessageListProps = {
  messages: ChatMessage[]
  onReply: (message: ChatMessage) => void
  onReact: (message: ChatMessage, type: MessageReactionType) => void
  isGroup?: boolean
}

export const MessageList = ({
  messages,
  onReply,
  onReact,
  isGroup = false,
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
          {messages.map((message, index) => {
            const nextMessage = messages[index + 1]
            const showTimestamp =
              !nextMessage ||
              nextMessage.senderUserId !== message.senderUserId ||
              !isSameMinute(message.sentAt, nextMessage.sentAt)

            return (
              <MessageBubble
                key={message.id}
                message={message}
                showSenderAvatar={!message.isMine}
                showSenderName={isGroup && !message.isMine}
                showReactionDetails={isGroup}
                showTimestamp={showTimestamp}
                onReply={() => onReply(message)}
                onReact={(type) => onReact(message, type)}
                onNavigateToMessage={navigateToMessage}
                isHighlighted={highlightedMessageId === message.id}
              />
            )
          })}

          {/* {messages.length > 0 && (
            <p className="pr-2 text-right text-xs text-muted-foreground">
              Delivered
            </p>
          )} */}
        </div>
      </div>
    </section>
  )
}

const isSameMinute = (first: string, second: string) =>
  Math.floor(new Date(first).getTime() / 60_000) ===
  Math.floor(new Date(second).getTime() / 60_000)
