import { apiClient } from "@/lib/api-client"
import { useQuery } from "@tanstack/react-query"

export type ChatUser = {
  id: string
  username: string
  fullName: string | null
  avatarUrl: string | null
}

export type LastMessage = {
  id: string
  senderUserId: string
  content: string
  sentAt: string
}

export type Conversation = {
  id: string
  type: "direct"
  otherUser: ChatUser
  lastMessage: LastMessage | null
  lastMessageAt: string | null
  unreadCount: number
}

export const chatQueryKeys = {
  all: ["chat"] as const,
  conversations: () => [...chatQueryKeys.all, "conversations"] as const,
}

export const getConversations = async () => {
  return apiClient.get<Conversation[]>("/chat/conversations")
}

export const useConversationsQuery = () => {
  return useQuery({
    queryKey: chatQueryKeys.conversations(),
    queryFn: getConversations,
  })
}
