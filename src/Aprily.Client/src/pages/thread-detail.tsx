import { MessageComposer } from "@/components/thread-details/message-composer"
import { MessageList } from "@/components/thread-details/message-list"
import { ThreadHeader } from "@/components/thread-details/thread-header"
import {
  useConversationMessagesQuery,
  useConversationsQuery,
  useMarkConversationAsReadMutation,
  useSendDirectMessageMutation,
  useSetMessageReactionMutation,
  type ChatMessage,
  type MessageReactionType,
} from "@/lib/chat-api"
import { useParams } from "@tanstack/react-router"
import { LoaderCircle } from "lucide-react"
import { useEffect, useRef, useState } from "react"

export const ThreadDetailPage = () => {
  const { threadId } = useParams({
    from: "/_authenticated/threads/$threadId",
  })

  const conversationsQuery = useConversationsQuery()
  const messagesQuery = useConversationMessagesQuery(threadId)
  const sendMessageMutation = useSendDirectMessageMutation()
  const markAsReadMutation = useMarkConversationAsReadMutation()
  const setReactionMutation = useSetMessageReactionMutation()
  const [replyingTo, setReplyingTo] = useState<ChatMessage | null>(null)
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

  const handleSendMessage = async (content: string, images: File[]) => {
    if (!conversation) {
      return
    }

    await sendMessageMutation.mutateAsync({
      conversationId: conversation.id,
      content,
      images,
      replyToMessageId: replyingTo?.id,
    })
    setReplyingTo(null)
  }

  const handleReact = (message: ChatMessage, type: MessageReactionType) => {
    const isRemoving = message.reactions.some(
      (reaction) => reaction.type === type && reaction.reactedByMe
    )

    setReactionMutation.mutate({
      messageId: message.id,
      type: isRemoving ? null : type,
    })
  }

  return (
    <main className="flex h-dvh w-dvw max-w-full flex-col overflow-hidden bg-background">
      <ThreadHeader
        thread={{
          avatarUrl: conversation?.avatarUrl ?? null,
          name: conversation?.name || "Chat",
          subtitle:
            conversation?.type === "group"
              ? `${conversation.memberCount} members`
              : undefined,
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
        <MessageList
          messages={messages}
          onReply={setReplyingTo}
          onReact={handleReact}
          isGroup={conversation?.type === "group"}
        />
      )}

      <MessageComposer
        onSend={handleSendMessage}
        replyingTo={replyingTo}
        onCancelReply={() => setReplyingTo(null)}
        disabled={!conversation || sendMessageMutation.isPending}
      />
    </main>
  )
}
