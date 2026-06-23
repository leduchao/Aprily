import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { ApiError, useSignInMutation } from "@/lib/auth-api"
import { useAuthStore } from "@/lib/auth-store"
import { zodResolver } from "@hookform/resolvers/zod"
import { Link, useNavigate } from "@tanstack/react-router"
import { Eye, EyeOff, LoaderCircle, LogIn } from "lucide-react"
import { useState } from "react"
import { useForm } from "react-hook-form"
import { toast } from "sonner"
import { z } from "zod"

const signInSchema = z.object({
  email: z.email(),
  password: z.string().min(1, "Password is required"),
})

type SignInForm = z.infer<typeof signInSchema>

export const SignInPage = () => {
  const navigate = useNavigate()
  const setSession = useAuthStore((state) => state.setSession)
  const signInMutation = useSignInMutation()
  const [showPassword, setShowPassword] = useState(false)

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<SignInForm>({
    resolver: zodResolver(signInSchema),
    defaultValues: {
      email: "",
      password: "",
    },
  })

  const onSubmit = handleSubmit((values) => {
    signInMutation.mutate(
      {
        email: values.email.trim(),
        password: values.password,
      },
      {
        onSuccess: (session) => {
          setSession(session.accessToken, session.user)
          void navigate({ to: "/" })
        },
        onError: (error) => {
          toast.error(
            error instanceof ApiError ? error.message : "Could not sign you in"
          )
        },
      }
    )
  })

  return (
    <main className="flex h-dvh w-dvw max-w-full flex-col overflow-hidden bg-background">
      <section className="flex min-h-0 flex-1 flex-col justify-center px-5 py-6">
        <div className="mb-8">
          <p className="font-medium text-muted-foreground">Aprily</p>
          <h1 className="mt-2 text-4xl font-bold tracking-normal">
            Welcome back
          </h1>
          <p className="mt-3 text-sm leading-6 text-muted-foreground">
            Sign in to keep your conversations close.
          </p>
        </div>

        <form className="space-y-4" onSubmit={onSubmit}>
          <div className="space-y-1.5">
            <label className="text-sm font-medium" htmlFor="email">
              Email
            </label>
            <Input
              id="email"
              type="email"
              autoComplete="email"
              className="h-12 rounded-full px-4"
              aria-invalid={Boolean(errors.email)}
              {...register("email")}
            />
            {errors.email && (
              <p className="text-xs text-destructive">{errors.email.message}</p>
            )}
          </div>

          <div className="space-y-1.5">
            <label className="text-sm font-medium" htmlFor="password">
              Password
            </label>
            <div className="relative">
              <Input
                id="password"
                type={showPassword ? "text" : "password"}
                autoComplete="current-password"
                className="h-12 rounded-full px-4 pr-12"
                aria-invalid={Boolean(errors.password)}
                {...register("password")}
              />
              <Button
                tabIndex={-1}
                type="button"
                variant="ghost"
                size="icon"
                className="absolute top-1/2 right-1.5 -translate-y-1/2 rounded-full"
                onClick={() => setShowPassword((value) => !value)}
                aria-label={showPassword ? "Hide password" : "Show password"}
              >
                {showPassword ? <EyeOff /> : <Eye />}
              </Button>
            </div>
            {errors.password && (
              <p className="text-xs text-destructive">
                {errors.password.message}
              </p>
            )}
          </div>

          <Button
            className="mt-2 h-12 w-full rounded-full text-base"
            size="lg"
            disabled={signInMutation.isPending}
          >
            {signInMutation.isPending ? (
              <LoaderCircle className="animate-spin" />
            ) : (
              <LogIn />
            )}
            Sign in
          </Button>
        </form>
      </section>

      <footer className="shrink-0 px-5 pb-6 text-center text-sm text-muted-foreground">
        New to Aprily?{" "}
        <Link className="font-medium text-foreground" to="/sign-up">
          Create account
        </Link>
      </footer>
    </main>
  )
}
