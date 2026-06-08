import { Button } from "@/components/ui/button"
import { Link } from "@tanstack/react-router"

export const HomePage = () => {
  return (
    <main className="flex min-h-screen flex-col items-center justify-center gap-4">
      <h1 className="text-3xl font-bold">Home Page</h1>

      <Button asChild>
        <Link to="/login">Go to Login</Link>
      </Button>
    </main>
  )
}
