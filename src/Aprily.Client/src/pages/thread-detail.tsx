import { MessageComposer } from "@/components/thread-details/message-composer"
import { MessageList } from "@/components/thread-details/message-list"
import { ThreadHeader } from "@/components/thread-details/thread-header"
import {
  type ChatMessage,
  getThreadById,
  getThreadMessages,
} from "@/data/threads"
import { useParams } from "@tanstack/react-router"
import { useState } from "react"

export const ThreadDetailPage = () => {
  const { threadId } = useParams({
    from: "/_authenticated/threads/$threadId",
  })
  const thread = getThreadById(threadId) ?? getThreadById("zaire-dorwart")
  const [messagesByThread, setMessagesByThread] = useState<
    Record<string, ChatMessage[]>
  >({})
  const messages = messagesByThread[threadId] ?? getThreadMessages(threadId)

  const handleSendMessage = (body: string) => {
    setMessagesByThread((currentMessagesByThread) => {
      const currentMessages =
        currentMessagesByThread[threadId] ?? getThreadMessages(threadId)

      return {
        ...currentMessagesByThread,
        [threadId]: [
          ...currentMessages.filter((message) => message.sender !== "typing"),
          {
            id: `local-${Date.now()}`,
            body,
            sender: "me",
          },
        ],
      }
    })
  }

  if (!thread) {
    return null
  }

  return (
    <main className="flex h-dvh w-dvw max-w-full flex-col overflow-hidden bg-background">
      <ThreadHeader thread={thread} />
      <MessageList messages={messages} />
      <MessageComposer onSend={handleSendMessage} />
    </main>
  )
}
