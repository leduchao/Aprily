import { refreshToken } from "@/lib/auth-api"
import { useAuthStore } from "@/lib/auth-store"
import { redirect } from "@tanstack/react-router"

export const requireAuth = async () => {
  const accessToken = useAuthStore.getState().accessToken

  if (accessToken) {
    return
  }

  try {
    const session = await refreshToken()
    useAuthStore.getState().setSession(session.accessToken, session.user)
  } catch {
    throw redirect({
      to: "/sign-in",
    })
  }
}

export const requireGuest = async () => {
  const accessToken = useAuthStore.getState().accessToken

  if (accessToken) {
    throw redirect({
      to: "/",
    })
  }

  try {
    const session = await refreshToken()
    useAuthStore.getState().setSession(session.accessToken, session.user)

    throw redirect({
      to: "/",
    })
  } catch (error) {
    if (isRedirect(error)) {
      throw error
    }
  }
}

const isRedirect = (error: unknown) => {
  return (
    typeof error === "object" &&
    error !== null &&
    "isRedirect" in error
  )
}
