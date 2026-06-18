import { useAuthStore } from "@/lib/auth-store"
import { redirect } from "@tanstack/react-router"

export const requireAuth = () => {
  const accessToken = useAuthStore.getState().accessToken

  if (!accessToken) {
    throw redirect({
      to: "/sign-in",
    })
  }
}
