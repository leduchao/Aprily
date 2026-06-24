import { ThreadItem } from "@/components/home/thread-item"
import { type Conversation, useConversationsQuery } from "@/lib/chat-api"
import { useAuthStore } from "@/lib/auth-store"
import { cn } from "@/lib/utils"
import { Link } from "@tanstack/react-router"
import { Ellipsis, LoaderCircle } from "lucide-react"

export const ThreadList = () => {
  const conversationsQuery = useConversationsQuery()
  const currentUserId = useAuthStore((state) => state.user?.id)

  return (
    <section className="min-h-0 flex-1 scrollbar-none overflow-x-hidden overflow-y-auto">
      <div className="sticky top-0 z-10 flex justify-between border-b border-border/60 bg-background px-4 py-2">
        <p className="text-xl font-medium">Chats</p>
        <Ellipsis />
      </div>

      <div className="bg-background shadow-sm">
        {conversationsQuery.isLoading && (
          <div className="flex items-center gap-2 px-4 py-6 text-sm text-muted-foreground">
            <LoaderCircle className="animate-spin" />
            Loading chats
          </div>
        )}

        {conversationsQuery.isError && (
          <p className="px-4 py-6 text-sm text-muted-foreground">
            Could not load conversations.
          </p>
        )}

        {conversationsQuery.data?.length === 0 && (
          <p className="px-4 py-6 text-sm text-muted-foreground">
            No conversations yet.
          </p>
        )}

        {conversationsQuery.data?.map((conversation, index) => (
          <Link
            key={conversation.id}
            to="/threads/$threadId"
            params={{ threadId: conversation.id }}
            className={cn("block", index > 0 && "border-t border-border/60")}
          >
            <ThreadItem
              id={conversation.id}
              avatar={conversation.avatarUrl ?? ""}
              name={conversation.name}
              message={formatLastMessage(conversation, currentUserId)}
              time={formatConversationTime(conversation.lastMessageAt)}
              unreadCount={conversation.unreadCount}
            />
          </Link>
        ))}
      </div>
    </section>
  )
}

const formatLastMessage = (
  conversation: Conversation,
  currentUserId?: string
) => {
  if (!conversation.lastMessage) {
    return "Say hello"
  }

  const message =
    conversation.lastMessage.content ||
    (conversation.lastMessage.hasAttachments ? "📷 Photo" : "Message")

  return conversation.lastMessage.senderUserId === currentUserId
    ? `You: ${message}`
    : message
}

const formatConversationTime = (value: string | null) => {
  if (!value) {
    return ""
  }

  return new Intl.DateTimeFormat(undefined, {
    hour: "2-digit",
    minute: "2-digit",
  }).format(new Date(value))
}
