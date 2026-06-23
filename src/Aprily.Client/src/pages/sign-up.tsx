import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { ApiError, useSignUpMutation } from "@/lib/auth-api"
import { useAuthStore } from "@/lib/auth-store"
import { zodResolver } from "@hookform/resolvers/zod"
import { Link, useNavigate } from "@tanstack/react-router"
import { Eye, EyeOff, LoaderCircle, UserPlus } from "lucide-react"
import { useState } from "react"
import { useForm } from "react-hook-form"
import { toast } from "sonner"
import { z } from "zod"

const signUpSchema = z
  .object({
    fullName: z.string().max(100).optional(),
    username: z.string().min(3).max(20),
    email: z.email(),
    password: z.string().min(6).max(16),
    confirmPassword: z.string().min(1, "Please confirm your password"),
  })
  .refine((values) => values.password === values.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"],
  })

type SignUpForm = z.infer<typeof signUpSchema>

export const SignUpPage = () => {
  const navigate = useNavigate()
  const setSession = useAuthStore((state) => state.setSession)
  const signUpMutation = useSignUpMutation()
  const [showPassword, setShowPassword] = useState(false)
  const [showConfirmPassword, setShowConfirmPassword] = useState(false)

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<SignUpForm>({
    resolver: zodResolver(signUpSchema),
    defaultValues: {
      fullName: "",
      username: "",
      email: "",
      password: "",
      confirmPassword: "",
    },
  })

  const onSubmit = handleSubmit((values) => {
    signUpMutation.mutate(
      {
        fullName: values.fullName?.trim() || undefined,
        username: values.username.trim(),
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
            error instanceof ApiError
              ? error.message
              : "Could not create your account"
          )
        },
      }
    )
  })

  return (
    <main className="flex h-dvh w-dvw max-w-full flex-col overflow-hidden bg-background">
      <section className="min-h-0 flex-1 overflow-y-auto px-5 py-6">
        <div className="mb-8">
          <p className="font-medium text-muted-foreground">Aprily</p>
          <h1 className="mt-2 text-4xl font-bold tracking-normal">
            Create account
          </h1>
          <p className="mt-3 text-sm leading-6 text-muted-foreground">
            Start a quieter place for close conversations.
          </p>
        </div>

        <form className="space-y-4" onSubmit={onSubmit}>
          <div className="space-y-2">
            <label className="text-sm font-medium" htmlFor="fullName">
              Full name
            </label>
            <Input
              id="fullName"
              autoComplete="name"
              className="h-12 rounded-full px-4"
              aria-invalid={Boolean(errors.fullName)}
              {...register("fullName")}
            />
            {errors.fullName && (
              <p className="text-xs text-destructive">
                {errors.fullName.message}
              </p>
            )}
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium" htmlFor="username">
              Username
            </label>
            <Input
              id="username"
              autoComplete="username"
              className="h-12 rounded-full px-4"
              aria-invalid={Boolean(errors.username)}
              {...register("username")}
            />
            {errors.username && (
              <p className="text-xs text-destructive">
                {errors.username.message}
              </p>
            )}
          </div>

          <div className="space-y-2">
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

          <div className="space-y-2">
            <label className="text-sm font-medium" htmlFor="password">
              Password
            </label>
            <div className="relative">
              <Input
                id="password"
                type={showPassword ? "text" : "password"}
                autoComplete="new-password"
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

          <div className="space-y-2">
            <label className="text-sm font-medium" htmlFor="confirmPassword">
              Confirm password
            </label>
            <div className="relative">
              <Input
                id="confirmPassword"
                type={showConfirmPassword ? "text" : "password"}
                autoComplete="new-password"
                className="h-12 rounded-full px-4 pr-12"
                aria-invalid={Boolean(errors.confirmPassword)}
                {...register("confirmPassword")}
              />
              <Button
                tabIndex={-1}
                type="button"
                variant="ghost"
                size="icon"
                className="absolute top-1/2 right-1.5 -translate-y-1/2 rounded-full"
                onClick={() => setShowConfirmPassword((value) => !value)}
                aria-label={
                  showConfirmPassword
                    ? "Hide confirm password"
                    : "Show confirm password"
                }
              >
                {showConfirmPassword ? <EyeOff /> : <Eye />}
              </Button>
            </div>
            {errors.confirmPassword && (
              <p className="text-xs text-destructive">
                {errors.confirmPassword.message}
              </p>
            )}
          </div>

          <Button
            className="mt-2 h-12 w-full rounded-full text-base"
            size="lg"
            disabled={signUpMutation.isPending}
          >
            {signUpMutation.isPending ? (
              <LoaderCircle className="animate-spin" />
            ) : (
              <UserPlus />
            )}
            Sign up
          </Button>
        </form>
      </section>

      <footer className="shrink-0 px-5 pb-6 text-center text-sm text-muted-foreground">
        Already have an account?{" "}
        <Link className="font-medium text-foreground" to="/sign-in">
          Sign in
        </Link>
      </footer>
    </main>
  )
}
