import { Button } from "@/components/ui/button"
import { Link } from "@tanstack/react-router"

export default function LoginPage() {
  return (
    <main className="flex min-h-screen flex-col items-center justify-center gap-4">
      <h1 className="text-3xl font-bold">Login Page</h1>

      <Button variant="outline" asChild>
        <Link to="/">Back Home</Link>
      </Button>
    </main>
  )
}
