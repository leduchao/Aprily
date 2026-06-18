import { requireGuest } from "@/lib/route-guards"
import { createFileRoute, Outlet } from "@tanstack/react-router"

export const Route = createFileRoute("/_guest")({
  beforeLoad: requireGuest,
  component: Outlet,
})
