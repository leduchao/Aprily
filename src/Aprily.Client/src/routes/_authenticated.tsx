import { AuthenticatedLayout } from "@/components/layout/authenticated-layout"
import { requireAuth } from "@/lib/route-guards"
import { createFileRoute } from "@tanstack/react-router"

export const Route = createFileRoute("/_authenticated")({
  beforeLoad: requireAuth,
  component: AuthenticatedLayout,
})
