import { SignInPage } from "@/pages/sign-in"
import { createFileRoute } from "@tanstack/react-router"

export const Route = createFileRoute("/_guest/sign-in")({
  component: SignInPage,
})
