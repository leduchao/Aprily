import { MessageComposer } from "@/components/thread-details/message-composer"
import { MessageList } from "@/components/thread-details/message-list"
import { ThreadHeader } from "@/components/thread-details/thread-header"
import {
  useConversationMessagesQuery,
  useConversationsQuery,
  useMarkConversationAsReadMutation,
  useSendDirectMessageMutation,
} from "@/lib/chat-api"
import { useParams } from "@tanstack/react-router"
import { LoaderCircle } from "lucide-react"
import { useEffect, useRef } from "react"

export const ThreadDetailPage = () => {
  const { threadId } = useParams({
    from: "/_authenticated/threads/$threadId",
  })

  const conversationsQuery = useConversationsQuery()
  const messagesQuery = useConversationMessagesQuery(threadId)
  const sendMessageMutation = useSendDirectMessageMutation()
  const markAsReadMutation = useMarkConversationAsReadMutation()
  const lastMarkedMessageIdRef = useRef<string | null>(null)

  const conversation = conversationsQuery.data?.find(
    (item) => item.id === threadId
  )

  const messages = [...(messagesQuery.data ?? [])].reverse()
  const latestMessage = messagesQuery.data?.[0]

  useEffect(() => {
    if (!latestMessage) {
      return
    }

    if (lastMarkedMessageIdRef.current === latestMessage.id) {
      return
    }

    lastMarkedMessageIdRef.current = latestMessage.id
    markAsReadMutation.mutate({
      conversationId: threadId,
      messageId: latestMessage.id,
    })
  }, [latestMessage, markAsReadMutation, threadId])

  const handleSendMessage = (content: string) => {
    if (!conversation) {
      return
    }

    sendMessageMutation.mutate({
      recipientUserId: conversation.otherUser.id,
      content,
    })
  }

  return (
    <main className="flex h-dvh w-dvw max-w-full flex-col overflow-hidden bg-background">
      <ThreadHeader
        thread={{
          avatarUrl: conversation?.otherUser.avatarUrl ?? null,
          name:
            conversation?.otherUser.fullName ||
            conversation?.otherUser.username ||
            "Chat",
        }}
      />

      {messagesQuery.isLoading ? (
        <section className="flex min-h-0 flex-1 items-center justify-center gap-2 text-sm text-muted-foreground">
          <LoaderCircle className="animate-spin" />
          Loading messages
        </section>
      ) : messagesQuery.isError ? (
        <section className="flex min-h-0 flex-1 items-center justify-center px-6 text-center text-sm text-muted-foreground">
          Could not load this conversation.
        </section>
      ) : (
        <MessageList messages={messages} />
      )}

      <MessageComposer
        onSend={handleSendMessage}
        disabled={!conversation || sendMessageMutation.isPending}
      />
    </main>
  )
}
