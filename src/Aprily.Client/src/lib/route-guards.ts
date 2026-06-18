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
