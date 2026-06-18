import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar"
import { Button } from "@/components/ui/button"
import { useSignOutMutation } from "@/lib/auth-api"
import { useAuthStore } from "@/lib/auth-store"
import { cn } from "@/lib/utils"
import { useQueryClient } from "@tanstack/react-query"
import { useNavigate } from "@tanstack/react-router"
import { useRef, useState, type ComponentType, type ReactNode } from "react"
import {
  ArrowLeft,
  BadgeCheck,
  CalendarClock,
  Copy,
  LoaderCircle,
  LogOut,
  Mail,
  UserRound,
} from "lucide-react"

export const ProfilePage = () => {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const user = useAuthStore((state) => state.user)
  const clearSession = useAuthStore((state) => state.clearSession)
  const signOutMutation = useSignOutMutation()
  const [isCopyNoticeVisible, setIsCopyNoticeVisible] = useState(false)
  const copyNoticeTimeoutRef = useRef<number | null>(null)

  const displayName = user?.fullName || user?.username || "Aprily user"

  const handleSignOut = () => {
    signOutMutation.mutate(undefined, {
      onSettled: () => {
        clearSession()
        queryClient.clear()
        void navigate({ to: "/sign-in", replace: true })
      },
    })
  }

  const handleCopyUserId = async () => {
    if (!user?.id) {
      return
    }

    await navigator.clipboard.writeText(user.id)
    setIsCopyNoticeVisible(true)

    if (copyNoticeTimeoutRef.current) {
      window.clearTimeout(copyNoticeTimeoutRef.current)
    }

    copyNoticeTimeoutRef.current = window.setTimeout(() => {
      setIsCopyNoticeVisible(false)
      copyNoticeTimeoutRef.current = null
    }, 1600)
  }

  return (
    <main className="flex h-dvh w-dvw max-w-full flex-col overflow-hidden bg-background">
      <header className="flex shrink-0 items-center justify-between border-b border-border/60 px-4 py-3">
        <Button
          variant="ghost"
          size="icon-lg"
          className="size-10 rounded-full"
          onClick={() => void navigate({ to: "/" })}
        >
          <ArrowLeft className="size-6" />
        </Button>
        <h1 className="text-lg font-semibold">Profile</h1>
        <div className="size-8" />
      </header>

      <section className="min-h-0 flex-1 overflow-y-auto px-5 py-6">
        <div className="flex flex-col items-center text-center">
          <Avatar className="size-24">
            <AvatarImage src={user?.avatarUrl ?? undefined} alt={displayName} />
            <AvatarFallback className="text-2xl">
              {getFallback(displayName)}
            </AvatarFallback>
          </Avatar>

          <h2 className="mt-4 max-w-full truncate text-2xl font-semibold">
            {displayName}
          </h2>
          <p className="mt-1 max-w-full truncate text-sm text-muted-foreground">
            @{user?.username ?? "user"}
          </p>
        </div>

        <div className="mt-8 overflow-hidden rounded-2xl border border-border/70 bg-card">
          <ProfileRow
            icon={Mail}
            label="Email"
            value={user?.email ?? "Unknown"}
          />
          <ProfileRow
            icon={BadgeCheck}
            label="Email status"
            value={user?.isEmailVerified ? "Verified" : "Not verified"}
            valueClassName={user?.isEmailVerified ? "text-primary" : undefined}
          />
          <ProfileRow
            icon={CalendarClock}
            label="Last sign in"
            value={formatDate(user?.lastSignInAt)}
          />
          <ProfileRow
            icon={UserRound}
            label="User id"
            value={user?.id ?? "Unknown"}
            action={
              <Button
                variant="ghost"
                size="icon-sm"
                className="rounded-full"
                onClick={handleCopyUserId}
                title="Copy user id"
              >
                <Copy />
              </Button>
            }
          />
        </div>

        <Button
          variant="destructive"
          size="lg"
          className="mt-6 h-12 w-full rounded-full text-base"
          disabled={signOutMutation.isPending}
          onClick={handleSignOut}
        >
          {signOutMutation.isPending ? (
            <LoaderCircle className="animate-spin" />
          ) : (
            <LogOut />
          )}
          Sign out
        </Button>
      </section>

      <div
        aria-live="polite"
        className={cn(
          "pointer-events-none fixed right-5 bottom-5 rounded-full bg-foreground px-4 py-2 text-sm font-medium text-background shadow-xl transition-all",
          isCopyNoticeVisible
            ? "translate-y-0 opacity-100"
            : "translate-y-2 opacity-0"
        )}
      >
        User id copied
      </div>
    </main>
  )
}

type ProfileRowProps = {
  icon: ComponentType<{ className?: string }>
  label: string
  value: string
  valueClassName?: string
  action?: ReactNode
}

const ProfileRow = ({
  icon: Icon,
  label,
  value,
  valueClassName,
  action,
}: ProfileRowProps) => {
  return (
    <div className="flex min-h-16 items-center gap-3 border-t border-border/60 px-4 first:border-t-0">
      <Icon className="size-5 shrink-0 text-muted-foreground" />
      <div className="min-w-0 flex-1">
        <p className="text-xs text-muted-foreground">{label}</p>
        <p className={cn("truncate text-sm font-medium", valueClassName)}>
          {value}
        </p>
      </div>
      {action}
    </div>
  )
}

const getFallback = (name: string) => {
  return name
    .split(" ")
    .map((part) => part[0])
    .join("")
    .slice(0, 2)
    .toUpperCase()
}

const formatDate = (value?: string) => {
  if (!value) {
    return "Unknown"
  }

  return new Intl.DateTimeFormat("en", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value))
}
