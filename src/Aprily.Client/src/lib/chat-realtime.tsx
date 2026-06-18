import type { PropsWithChildren } from "react"
import { useEffect } from "react"

import {
  chatQueryKeys,
  type ChatMessage,
  type Conversation,
} from "@/lib/chat-api"
import { useAuthStore } from "@/lib/auth-store"
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr"
import { useQueryClient } from "@tanstack/react-query"

const configuredBaseUrl = import.meta.env.VITE_API_BASE_URL ?? "/api"

export const ChatRealtimeProvider = ({ children }: PropsWithChildren) => {
  const accessToken = useAuthStore((state) => state.accessToken)
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

    void connection.start().catch((error: unknown) => {
      console.error("Unable to connect to chat realtime hub", error)
    })

    return () => {
      connection.off("messageReceived")
      connection.off("conversationUpdated")
      void connection.stop()
    }
  }, [accessToken, queryClient])

  return children
}

const resolveHubUrl = () => {
  const normalizedBaseUrl = configuredBaseUrl.replace(/\/+$/, "")
  const apiBaseUrl = normalizedBaseUrl.replace(/\/api(?:\/v1)?$/, "")

  return `${apiBaseUrl}/hubs/chat`
}
