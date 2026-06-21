import { apiClient } from "@/lib/api-client"
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"

export type ChatUser = {
  id: string
  username: string
  fullName: string | null
  avatarUrl: string | null
}

export type LastMessage = {
  id: string
  senderUserId: string
  content: string | null
  hasAttachments: boolean
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

export type ChatMessage = {
  id: string
  conversationId: string
  senderUserId: string
  senderUsername: string
  senderAvatarUrl: string | null
  content: string | null
  attachments: ChatMessageAttachment[]
  replyTo: ChatMessageReply | null
  reactions: MessageReactionSummary[]
  sentAt: string
  isMine: boolean
}

export type ChatMessageReply = {
  id: string
  senderUserId: string
  senderUsername: string
  content: string | null
  hasAttachments: boolean
}

export type MessageReactionType =
  | "like"
  | "love"
  | "haha"
  | "sad"
  | "wow"
  | "angry"

export type MessageReactionSummary = {
  type: MessageReactionType
  count: number
  reactedByMe: boolean
}

export type MessageReactionsUpdated = {
  conversationId: string
  messageId: string
  actorUserId: string
  reactions: MessageReactionSummary[]
}

export type ChatMessageAttachment = {
  id: string
  type: "image"
  url: string
  originalFileName: string | null
  contentType: string
  sizeBytes: number
  width: number | null
  height: number | null
  sortOrder: number
}

export type SendDirectMessageInput = {
  recipientUserId: string
  content: string
  images?: File[]
  replyToMessageId?: string
}

export type SendDirectMessageResponse = {
  conversationId: string
  message: ChatMessage
}

export type OpenDirectConversationResponse = {
  conversationId: string
}

export type MarkConversationAsReadInput = {
  conversationId: string
  messageId: string
}

export type MarkConversationAsReadResponse = {
  conversationId: string
  lastReadMessageId: string
  lastReadAt: string
}

export const chatQueryKeys = {
  all: ["chat"] as const,
  conversations: () => [...chatQueryKeys.all, "conversations"] as const,
  messages: (conversationId: string) =>
    [...chatQueryKeys.all, "messages", conversationId] as const,
}

export const getConversations = async () => {
  return apiClient.get<Conversation[]>("/chat/conversations")
}

export const getConversationMessages = async (conversationId: string) => {
  return apiClient.get<ChatMessage[]>(
    `/chat/conversations/${conversationId}/messages`
  )
}

export const openDirectConversation = async (recipientUserId: string) => {
  return apiClient.post<
    OpenDirectConversationResponse,
    { recipientUserId: string }
  >("/chat/direct-conversations", { recipientUserId })
}

export const sendDirectMessage = async (input: SendDirectMessageInput) => {
  if (input.images?.length) {
    const formData = new FormData()
    formData.append("recipientUserId", input.recipientUserId)
    formData.append("content", input.content)
    if (input.replyToMessageId) {
      formData.append("replyToMessageId", input.replyToMessageId)
    }
    input.images.forEach((image) => formData.append("images", image))

    return apiClient.post<SendDirectMessageResponse, FormData>(
      "/chat/direct-image-messages",
      formData
    )
  }

  return apiClient.post<SendDirectMessageResponse, SendDirectMessageInput>(
    "/chat/direct-messages",
    {
      recipientUserId: input.recipientUserId,
      content: input.content,
      replyToMessageId: input.replyToMessageId,
    }
  )
}

export const setMessageReaction = async (input: {
  messageId: string
  type: MessageReactionType | null
}) => {
  return apiClient.put<
    MessageReactionsUpdated,
    { type: MessageReactionType | null }
  >(`/chat/messages/${input.messageId}/reaction`, { type: input.type })
}

export const markConversationAsRead = async (
  input: MarkConversationAsReadInput
) => {
  return apiClient.post<MarkConversationAsReadResponse, { messageId: string }>(
    `/chat/conversations/${input.conversationId}/read`,
    {
      messageId: input.messageId,
    }
  )
}

export const useConversationsQuery = () => {
  return useQuery({
    queryKey: chatQueryKeys.conversations(),
    queryFn: getConversations,
  })
}

export const useConversationMessagesQuery = (conversationId: string) => {
  return useQuery({
    queryKey: chatQueryKeys.messages(conversationId),
    queryFn: () => getConversationMessages(conversationId),
  })
}

export const useOpenDirectConversationMutation = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: openDirectConversation,
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: chatQueryKeys.conversations(),
      })
    },
  })
}

export const useSendDirectMessageMutation = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: sendDirectMessage,
    onSuccess: async (response) => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: chatQueryKeys.conversations(),
        }),
        queryClient.invalidateQueries({
          queryKey: chatQueryKeys.messages(response.conversationId),
        }),
      ])
    },
  })
}

export const useSetMessageReactionMutation = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: setMessageReaction,
    onSuccess: (response) => {
      queryClient.setQueryData<ChatMessage[]>(
        chatQueryKeys.messages(response.conversationId),
        (messages) =>
          messages?.map((message) =>
            message.id === response.messageId
              ? { ...message, reactions: response.reactions }
              : message
          )
      )
    },
  })
}

export const useMarkConversationAsReadMutation = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: markConversationAsRead,
    onSuccess: (response) => {
      queryClient.setQueryData<Conversation[]>(
        chatQueryKeys.conversations(),
        (currentConversations) => {
          if (!currentConversations) {
            return currentConversations
          }

          return currentConversations.map((conversation) =>
            conversation.id === response.conversationId
              ? { ...conversation, unreadCount: 0 }
              : conversation
          )
        }
      )
    },
  })
}
