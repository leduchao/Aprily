import { SignUpPage } from "@/pages/sign-up"
import { createFileRoute } from "@tanstack/react-router"

export const Route = createFileRoute("/_guest/sign-up")({
  component: SignUpPage,
})
