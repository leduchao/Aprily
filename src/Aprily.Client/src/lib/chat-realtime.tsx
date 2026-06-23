import type { PropsWithChildren } from "react"
import { useEffect } from "react"

import {
  chatQueryKeys,
  type ChatMessage,
  type Conversation,
  type MessageReactionsUpdated,
} from "@/lib/chat-api"
import { useAuthStore } from "@/lib/auth-store"
import { friendQueryKeys } from "@/lib/friends-api"
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr"
import { useQueryClient } from "@tanstack/react-query"

const configuredBaseUrl = import.meta.env.VITE_API_BASE_URL ?? "/api"

export const ChatRealtimeProvider = ({ children }: PropsWithChildren) => {
  const accessToken = useAuthStore((state) => state.accessToken)
  const currentUserId = useAuthStore((state) => state.user?.id)
  const queryClient = useQueryClient()

  useEffect(() => {
    if (!accessToken) {
      return
    }

    const connection = new HubConnectionBuilder()
      .withUrl(resolveHubUrl(), {
        accessTokenFactory: () => accessToken,
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build()

    connection.on("messageReceived", (message: ChatMessage) => {
      queryClient.setQueryData<ChatMessage[]>(
        chatQueryKeys.messages(message.conversationId),
        (currentMessages) => {
          if (!currentMessages) {
            return currentMessages
          }

          if (currentMessages.some((item) => item.id === message.id)) {
            return currentMessages
          }

          return [message, ...currentMessages]
        }
      )

      queryClient.setQueryData<Conversation[]>(
        chatQueryKeys.conversations(),
        (currentConversations) => {
          if (!currentConversations) {
            return currentConversations
          }

          return currentConversations.map((conversation) => {
            if (conversation.id !== message.conversationId) {
              return conversation
            }

            return {
              ...conversation,
              lastMessage: {
                id: message.id,
                senderUserId: message.senderUserId,
                content: message.content,
                hasAttachments: message.attachments.length > 0,
                sentAt: message.sentAt,
              },
              lastMessageAt: message.sentAt,
              unreadCount: message.isMine
                ? conversation.unreadCount
                : conversation.unreadCount + 1,
            }
          })
        }
      )
    })

    connection.on("conversationUpdated", () => {
      void queryClient.invalidateQueries({
        queryKey: chatQueryKeys.conversations(),
      })
    })

    connection.on("friendRequestsUpdated", () => {
      void queryClient.invalidateQueries({ queryKey: friendQueryKeys.all })
    })

    connection.on(
      "messageReactionsUpdated",
      (response: MessageReactionsUpdated) => {
        queryClient.setQueryData<ChatMessage[]>(
          chatQueryKeys.messages(response.conversationId),
          (messages) =>
            messages?.map((message) => {
              if (message.id !== response.messageId) {
                return message
              }

              const reactions = response.reactions.map((reaction) => ({
                ...reaction,
                reactedByMe:
                  response.actorUserId === currentUserId
                    ? reaction.reactedByMe
                    : (message.reactions.find(
                        (currentReaction) =>
                          currentReaction.type === reaction.type
                      )?.reactedByMe ?? false),
              }))

              return { ...message, reactions }
            })
        )
      }
    )

    void connection.start().catch((error: unknown) => {
      console.error("Unable to connect to chat realtime hub", error)
    })

    return () => {
      connection.off("messageReceived")
      connection.off("conversationUpdated")
      connection.off("friendRequestsUpdated")
      connection.off("messageReactionsUpdated")
      void connection.stop()
    }
  }, [accessToken, currentUserId, queryClient])

  return children
}

const resolveHubUrl = () => {
  const normalizedBaseUrl = configuredBaseUrl.replace(/\/+$/, "")
  const apiBaseUrl = normalizedBaseUrl.replace(/\/api(?:\/v1)?$/, "")

  return `${apiBaseUrl}/hubs/chat`
}
