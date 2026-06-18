import { ChatRealtimeProvider } from "@/lib/chat-realtime"
import { Outlet } from "@tanstack/react-router"

export const AuthenticatedLayout = () => {
  return (
    <ChatRealtimeProvider>
      <Outlet />
    </ChatRealtimeProvider>
  )
}
