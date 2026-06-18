import { SignInPage } from "@/pages/sign-in"
import { createFileRoute } from "@tanstack/react-router"

export const Route = createFileRoute("/sign-in")({
  component: SignInPage,
})
