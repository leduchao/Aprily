import { ThreadDetailPage } from "@/pages/thread-detail"
import { createFileRoute } from "@tanstack/react-router"

export const Route = createFileRoute("/threads/$threadId")({
  component: ThreadDetailPage,
})
