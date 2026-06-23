import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar"
import { Button } from "@/components/ui/button"
import {
  ApiError,
  useSignOutMutation,
  useUpdateProfileMutation,
  useUploadAvatarMutation,
} from "@/lib/auth-api"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import { useAuthStore } from "@/lib/auth-store"
import { cn } from "@/lib/utils"
import { useQueryClient } from "@tanstack/react-query"
import { useNavigate } from "@tanstack/react-router"
import {
  useEffect,
  useRef,
  useState,
  type ChangeEvent,
  type ComponentType,
  type ReactNode,
} from "react"
import {
  ArrowLeft,
  BadgeCheck,
  CalendarClock,
  Camera,
  Copy,
  LoaderCircle,
  LogOut,
  Mail,
  Pencil,
  UserRound,
} from "lucide-react"

export const ProfilePage = () => {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const user = useAuthStore((state) => state.user)
  const setUser = useAuthStore((state) => state.setUser)
  const clearSession = useAuthStore((state) => state.clearSession)
  const signOutMutation = useSignOutMutation()
  const updateProfileMutation = useUpdateProfileMutation()
  const uploadAvatarMutation = useUploadAvatarMutation()
  const [isCopyNoticeVisible, setIsCopyNoticeVisible] = useState(false)
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false)
  const [fullName, setFullName] = useState(user?.fullName ?? "")
  const [avatarFile, setAvatarFile] = useState<File | null>(null)
  const [avatarPreviewUrl, setAvatarPreviewUrl] = useState<string | null>(null)
  const [fileError, setFileError] = useState<string | null>(null)
  const copyNoticeTimeoutRef = useRef<number | null>(null)
  const avatarInputRef = useRef<HTMLInputElement | null>(null)

  const displayName = user?.fullName || user?.username || "Aprily user"
  const isSaving =
    updateProfileMutation.isPending || uploadAvatarMutation.isPending
  const normalizedFullName = fullName.trim() || null
  const currentFullName = user?.fullName?.trim() || null
  const hasFullNameChanged = normalizedFullName !== currentFullName
  const hasProfileChanges = hasFullNameChanged || avatarFile !== null

  useEffect(() => {
    return () => {
      if (avatarPreviewUrl) {
        URL.revokeObjectURL(avatarPreviewUrl)
      }
    }
  }, [avatarPreviewUrl])

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

  const handleEditDialogChange = (open: boolean) => {
    if (isSaving) {
      return
    }

    setIsEditDialogOpen(open)
    if (open) {
      setFullName(user?.fullName ?? "")
      setAvatarFile(null)
      setAvatarPreviewUrl(null)
      setFileError(null)
      updateProfileMutation.reset()
      uploadAvatarMutation.reset()
    }
  }

  const handleAvatarChange = (event: ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0]
    event.target.value = ""

    if (!file) {
      return
    }

    const allowedTypes = ["image/jpeg", "image/png", "image/webp", "image/gif"]
    if (!allowedTypes.includes(file.type)) {
      setFileError("Choose a JPG, PNG, WEBP, or GIF image")
      return
    }

    if (file.size > 5 * 1024 * 1024) {
      setFileError("Avatar must be smaller than 5 MB")
      return
    }

    setFileError(null)
    setAvatarFile(file)
    setAvatarPreviewUrl(URL.createObjectURL(file))
  }

  const handleSaveProfile = async () => {
    if (fullName.trim().length > 100 || !hasProfileChanges) {
      return
    }

    try {
      if (hasFullNameChanged) {
        const updatedUser = await updateProfileMutation.mutateAsync({
          fullName: normalizedFullName,
        })
        setUser(updatedUser)
      }

      if (avatarFile) {
        const updatedUser = await uploadAvatarMutation.mutateAsync(avatarFile)
        setUser(updatedUser)
      }

      setIsEditDialogOpen(false)
    } catch {
      // Mutation state renders the API error in the dialog.
    }
  }

  const profileError =
    fileError ??
    getErrorMessage(updateProfileMutation.error) ??
    getErrorMessage(uploadAvatarMutation.error)

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
        <Button
          variant="ghost"
          size="icon-lg"
          className="size-10 rounded-full"
          onClick={() => handleEditDialogChange(true)}
          aria-label="Edit profile"
        >
          <Pencil className="size-5" />
        </Button>
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
            icon={UserRound}
            label="Full name"
            value={user?.fullName || "Not set"}
          />
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

      <Dialog open={isEditDialogOpen} onOpenChange={handleEditDialogChange}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Edit profile</DialogTitle>
            <DialogDescription>
              Update the name and photo people see across Aprily.
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-5">
            <div className="flex flex-col items-center gap-3">
              <button
                type="button"
                className="group relative rounded-full outline-none focus-visible:ring-3 focus-visible:ring-ring/30"
                onClick={() => avatarInputRef.current?.click()}
                aria-label="Choose a new avatar"
              >
                <Avatar className="size-24">
                  <AvatarImage
                    src={avatarPreviewUrl ?? user?.avatarUrl ?? undefined}
                    alt={fullName || displayName}
                  />
                  <AvatarFallback className="text-2xl">
                    {getFallback(fullName || displayName)}
                  </AvatarFallback>
                </Avatar>
                <span className="absolute inset-0 flex items-center justify-center rounded-full bg-black/45 text-white opacity-0 transition-opacity group-hover:opacity-100 group-focus-visible:opacity-100">
                  <Camera className="size-6" />
                </span>
              </button>
              <Button
                type="button"
                variant="ghost"
                size="sm"
                className="rounded-full"
                onClick={() => avatarInputRef.current?.click()}
              >
                Change photo
              </Button>
              <input
                ref={avatarInputRef}
                type="file"
                accept="image/jpeg,image/png,image/webp,image/gif"
                className="sr-only"
                onChange={handleAvatarChange}
              />
            </div>

            <div className="space-y-2">
              <label
                className="text-sm font-medium"
                htmlFor="profile-full-name"
              >
                Full name
              </label>
              <Input
                id="profile-full-name"
                value={fullName}
                maxLength={100}
                autoComplete="name"
                className="h-12 rounded-full px-4"
                onChange={(event) => setFullName(event.target.value)}
              />
              <p className="text-right text-xs text-muted-foreground">
                {fullName.length}/100
              </p>
            </div>

            {profileError && (
              <p role="alert" className="text-sm text-destructive">
                {profileError}
              </p>
            )}
          </div>

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              className="rounded-full"
              disabled={isSaving}
              onClick={() => handleEditDialogChange(false)}
            >
              Cancel
            </Button>
            <Button
              type="button"
              className="rounded-full"
              disabled={isSaving || Boolean(fileError) || !hasProfileChanges}
              onClick={() => void handleSaveProfile()}
            >
              {isSaving && <LoaderCircle className="animate-spin" />}
              Save changes
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
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

const getErrorMessage = (error: Error | null) => {
  if (!error) {
    return null
  }

  return error instanceof ApiError ? error.message : "Could not update profile"
}
