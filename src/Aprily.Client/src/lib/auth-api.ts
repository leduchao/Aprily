import { ApiError, apiClient } from "@/lib/api-client"
import { useMutation } from "@tanstack/react-query"

export type AuthUser = {
  id: string
  username: string
  fullName: string | null
  email: string
  avatarUrl: string | null
  lastSignInAt: string
  isEmailVerified: boolean
}

export type AuthPayload = {
  accessToken: string
  user: AuthUser
}

export type SignInRequest = {
  email: string
  password: string
}

export type SignUpRequest = {
  fullName?: string
  username: string
  email: string
  password: string
}

export type UpdateProfileRequest = {
  fullName: string | null
}

export const signIn = async (request: SignInRequest) => {
  return apiClient.post<AuthPayload, SignInRequest>(
    "/users/auth/sign-in",
    request,
    { auth: false }
  )
}

export const signUp = async (request: SignUpRequest) => {
  return apiClient.post<AuthPayload, SignUpRequest>(
    "/users/auth/sign-up",
    request,
    { auth: false }
  )
}

export const refreshToken = async () => {
  return apiClient.post<AuthPayload>("/users/auth/refresh-token", undefined, {
    auth: false,
  })
}

export const signOut = async () => {
  return apiClient.post<void>("/users/auth/sign-out", undefined, {
    auth: false,
  })
}

export const updateProfile = async (request: UpdateProfileRequest) => {
  return apiClient.patch<AuthUser, UpdateProfileRequest>("/users/me", request)
}

export const uploadAvatar = async (avatar: File) => {
  const formData = new FormData()
  formData.append("avatar", avatar)

  return apiClient.post<AuthUser, FormData>("/users/me/avatar", formData)
}

export const useSignInMutation = () => {
  return useMutation({
    mutationFn: signIn,
  })
}

export const useSignUpMutation = () => {
  return useMutation({
    mutationFn: signUp,
  })
}

export const useSignOutMutation = () => {
  return useMutation({
    mutationFn: signOut,
  })
}

export const useUpdateProfileMutation = () => {
  return useMutation({ mutationFn: updateProfile })
}

export const useUploadAvatarMutation = () => {
  return useMutation({ mutationFn: uploadAvatar })
}

export { ApiError }
