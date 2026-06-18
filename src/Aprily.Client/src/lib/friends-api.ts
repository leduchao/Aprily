import { apiClient } from "@/lib/api-client"
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"

export type FriendUser = {
  id: string
  username: string
  fullName: string | null
  email: string
  avatarUrl: string | null
}

export type Friend = {
  id: string
  user: FriendUser
  createdAt: string
}

export type FriendRequest = {
  id: string
  requester: FriendUser
  addressee: FriendUser
  status: "pending" | "accepted" | "declined" | "canceled"
  createdAt: string
  respondedAt: string | null
}

export type SendFriendRequestInput =
  | {
      email: string
      recipientUserId?: never
    }
  | {
      email?: never
      recipientUserId: string
    }

export const friendQueryKeys = {
  all: ["friends"] as const,
  list: () => [...friendQueryKeys.all, "list"] as const,
  requests: (direction: "incoming" | "outgoing", status = "pending") =>
    [...friendQueryKeys.all, "requests", direction, status] as const,
}

export const getFriends = async () => {
  return apiClient.get<Friend[]>("/friends/")
}

export const getFriendRequests = async (
  direction: "incoming" | "outgoing",
  status = "pending"
) => {
  const searchParams = new URLSearchParams({
    direction,
    status,
  })

  return apiClient.get<FriendRequest[]>(`/friends/requests?${searchParams}`)
}

export const sendFriendRequest = async (input: SendFriendRequestInput) => {
  return apiClient.post<FriendRequest, SendFriendRequestInput>(
    "/friends/requests",
    input
  )
}

export const acceptFriendRequest = async (requestId: string) => {
  return apiClient.post<FriendRequest>(`/friends/requests/${requestId}/accept`)
}

export const declineFriendRequest = async (requestId: string) => {
  return apiClient.post<FriendRequest>(`/friends/requests/${requestId}/decline`)
}

export const useFriendsQuery = () => {
  return useQuery({
    queryKey: friendQueryKeys.list(),
    queryFn: getFriends,
  })
}

export const useIncomingFriendRequestsQuery = () => {
  return useQuery({
    queryKey: friendQueryKeys.requests("incoming"),
    queryFn: () => getFriendRequests("incoming"),
  })
}

export const useSendFriendRequestMutation = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: sendFriendRequest,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: friendQueryKeys.all })
    },
  })
}

export const useAcceptFriendRequestMutation = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: acceptFriendRequest,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: friendQueryKeys.all })
    },
  })
}

export const useDeclineFriendRequestMutation = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: declineFriendRequest,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: friendQueryKeys.all })
    },
  })
}
