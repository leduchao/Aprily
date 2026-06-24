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
  type: "direct" | "group"
  name: string
  avatarUrl: string | null
  otherUser: ChatUser | null
  memberCount: number
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
  reactedBy: string[]
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
  conversationId: string
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

export type CreateGroupConversationInput = {
  name: string
  memberUserIds: string[]
}

export type GroupMember = ChatUser & {
  role: "owner" | "admin" | "member"
}

export type GroupConversationDetails = {
  conversationId: string
  name: string
  avatarUrl: string | null
  owner: ChatUser
  currentUserRole: "owner" | "admin" | "member"
  members: GroupMember[]
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
  conversationSearch: (query: string) =>
    [...chatQueryKeys.all, "conversations", "search", query] as const,
  groupInfo: (conversationId: string) =>
    [...chatQueryKeys.conversations(), "group", conversationId] as const,
  messages: (conversationId: string) =>
    [...chatQueryKeys.all, "messages", conversationId] as const,
}

export const getConversations = async () => {
  return apiClient.get<Conversation[]>("/chat/conversations")
}

export const searchConversations = async (query: string) => {
  const searchParams = new URLSearchParams({ query })

  return apiClient.get<Conversation[]>(
    `/chat/conversations/search?${searchParams}`
  )
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

export const createGroupConversation = async (
  input: CreateGroupConversationInput
) => {
  return apiClient.post<
    OpenDirectConversationResponse,
    CreateGroupConversationInput
  >("/chat/group-conversations", input)
}

export const getGroupConversationDetails = async (conversationId: string) =>
  apiClient.get<GroupConversationDetails>(
    `/chat/group-conversations/${conversationId}`
  )

export const updateGroupConversation = async (input: {
  conversationId: string
  name: string
}) =>
  apiClient.put<{ conversationId: string; name: string }, { name: string }>(
    `/chat/group-conversations/${input.conversationId}`,
    { name: input.name }
  )

export const addGroupMembers = async (input: {
  conversationId: string
  memberUserIds: string[]
}) =>
  apiClient.post<
    { conversationId: string; addedCount: number },
    { memberUserIds: string[] }
  >(`/chat/group-conversations/${input.conversationId}/members`, {
    memberUserIds: input.memberUserIds,
  })

export const sendDirectMessage = async (input: SendDirectMessageInput) => {
  if (input.images?.length) {
    const formData = new FormData()
    formData.append("content", input.content)
    if (input.replyToMessageId) {
      formData.append("replyToMessageId", input.replyToMessageId)
    }
    input.images.forEach((image) => formData.append("images", image))

    return apiClient.post<SendDirectMessageResponse, FormData>(
      `/chat/conversations/${input.conversationId}/image-messages`,
      formData
    )
  }

  return apiClient.post<SendDirectMessageResponse, SendDirectMessageInput>(
    `/chat/conversations/${input.conversationId}/messages`,
    {
      conversationId: input.conversationId,
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

export const useSearchConversationsQuery = (query: string) => {
  return useQuery({
    queryKey: chatQueryKeys.conversationSearch(query),
    queryFn: () => searchConversations(query),
    enabled: query.length > 0,
  })
}

export const useGroupConversationDetailsQuery = (
  conversationId: string,
  enabled = true
) =>
  useQuery({
    queryKey: chatQueryKeys.groupInfo(conversationId),
    queryFn: () => getGroupConversationDetails(conversationId),
    enabled: enabled && conversationId.length > 0,
  })

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

export const useCreateGroupConversationMutation = () => {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: createGroupConversation,
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: chatQueryKeys.conversations(),
      })
    },
  })
}

export const useUpdateGroupConversationMutation = () => {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: updateGroupConversation,
    onSuccess: async (response) => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: chatQueryKeys.conversations(),
        }),
        queryClient.invalidateQueries({
          queryKey: chatQueryKeys.groupInfo(response.conversationId),
        }),
      ])
    },
  })
}

export const useAddGroupMembersMutation = () => {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: addGroupMembers,
    onSuccess: async (response) => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: chatQueryKeys.conversations(),
        }),
        queryClient.invalidateQueries({
          queryKey: chatQueryKeys.groupInfo(response.conversationId),
        }),
      ])
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
