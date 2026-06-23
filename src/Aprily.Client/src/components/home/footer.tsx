import { useState, type FormEvent, type ReactNode } from "react"
import {
  ArrowLeft,
  ContactRound,
  Home,
  LoaderCircle,
  MessageSquare,
  Plus,
  UserRound,
} from "lucide-react"

import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { ApiError } from "@/lib/api-client"
import { useOpenDirectConversationMutation } from "@/lib/chat-api"
import {
  type FriendUser,
  useFriendsQuery,
  useSendFriendRequestMutation,
} from "@/lib/friends-api"
import { cn } from "@/lib/utils"
import { useNavigate } from "@tanstack/react-router"
import { toast } from "sonner"

type FooterMode = "menu" | "new-chat" | "new-contact"

const actions = [
  {
    mode: "new-chat" as const,
    icon: MessageSquare,
    title: "New Chat",
    description: "Choose a friend to start messaging",
  },
  {
    mode: "new-contact" as const,
    icon: ContactRound,
    title: "New Contact",
    description: "Add a contact by email or user id",
  },
]

export const Footer = () => {
  const [isOpen, setIsOpen] = useState(false)
  const [mode, setMode] = useState<FooterMode>("menu")
  const navigate = useNavigate()

  const handleOpenChange = (open: boolean) => {
    setIsOpen(open)
    if (!open) {
      setMode("menu")
    }
  }

  return (
    <Dialog open={isOpen} onOpenChange={handleOpenChange}>
      <footer className="shrink-0 border-t border-border/60 bg-background py-4">
        <div className="grid grid-cols-[1fr_auto_1fr] items-center">
          <div className="flex justify-center">
            <Button
              variant="ghost"
              size="icon"
              className="size-12 rounded-full"
              onClick={() => void navigate({ to: "/" })}
            >
              <Home className="size-7" />
            </Button>
          </div>

          <DialogTrigger asChild>
            <Button
              className={cn(
                "h-12 gap-2 rounded-full bg-primary px-8 text-lg font-medium",
                isOpen && "opacity-0"
              )}
            >
              <Plus className="size-6" />
              <span>New Chat</span>
            </Button>
          </DialogTrigger>

          <div className="flex justify-center">
            <Button
              variant="ghost"
              size="icon"
              className="size-12 rounded-full"
              title="Profile"
              onClick={() => void navigate({ to: "/profile" })}
            >
              <UserRound className="size-7" />
            </Button>
          </div>
        </div>
      </footer>

      <DialogContent
        showCloseButton={false}
        className="top-auto bottom-0 left-1/2 w-full max-w-md -translate-x-1/2 translate-y-0 gap-4 border-none bg-transparent p-4 shadow-none ring-0 data-open:zoom-in-100 data-closed:zoom-out-100"
      >
        <DialogTitle className="sr-only">Create a new chat</DialogTitle>
        <DialogDescription className="sr-only">
          Choose whether to start a chat, add a contact, or review friend
          requests.
        </DialogDescription>

        <div className="overflow-hidden rounded-3xl bg-card shadow-2xl">
          {mode === "menu" && <ActionMenu onSelect={setMode} />}
          {mode === "new-chat" && (
            <NewChatPanel
              onBack={() => setMode("menu")}
              onClose={() => handleOpenChange(false)}
            />
          )}
          {mode === "new-contact" && (
            <NewContactPanel onBack={() => setMode("menu")} />
          )}
        </div>

        <DialogClose asChild>
          <Button className="mx-auto h-12 min-w-48 rounded-full bg-card px-10 text-lg font-medium text-card-foreground shadow-xl hover:bg-card/90">
            Cancel
          </Button>
        </DialogClose>
      </DialogContent>
    </Dialog>
  )
}

const ActionMenu = ({ onSelect }: { onSelect: (mode: FooterMode) => void }) => {
  return (
    <>
      {actions.map((action, index) => {
        const Icon = action.icon

        return (
          <Button
            variant="ghost"
            key={action.title}
            className={cn(
              "h-auto w-full justify-start gap-4 rounded-none px-6 py-4 text-left hover:bg-muted/50",
              index > 0 && "border-t border-border/60"
            )}
            onClick={() => onSelect(action.mode)}
          >
            <Icon className="size-6 shrink-0 text-foreground" />
            <span className="min-w-0">
              <span className="block text-lg font-medium text-foreground">
                {action.title}
              </span>
              <span className="mt-1 block truncate text-sm text-muted-foreground">
                {action.description}
              </span>
            </span>
          </Button>
        )
      })}
    </>
  )
}

const NewContactPanel = ({ onBack }: { onBack: () => void }) => {
  const [identifier, setIdentifier] = useState("")
  const sendFriendRequestMutation = useSendFriendRequestMutation()

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()

    const value = identifier.trim()
    if (!value) {
      return
    }

    sendFriendRequestMutation.mutate(
      value.includes("@") ? { email: value } : { recipientUserId: value },
      {
        onSuccess: () => {
          setIdentifier("")
          toast.success("Friend request sent")
        },
        onError: (error) => {
          toast.error(
            error instanceof ApiError
              ? error.message
              : "Could not send friend request"
          )
        },
      }
    )
  }

  return (
    <div className="p-5">
      <PanelHeader title="New Contact" onBack={onBack} />

      <form className="mt-4 space-y-3" onSubmit={handleSubmit}>
        <div className="space-y-1.5">
          <label className="text-sm font-medium" htmlFor="friendIdentifier">
            Email or user id
          </label>
          <Input
            id="friendIdentifier"
            value={identifier}
            onChange={(event) => setIdentifier(event.target.value)}
            placeholder="friend@example.com"
            autoComplete="off"
          />
        </div>

        <Button
          className="w-full"
          size="lg"
          disabled={sendFriendRequestMutation.isPending}
        >
          {sendFriendRequestMutation.isPending ? (
            <LoaderCircle className="animate-spin" />
          ) : (
            <ContactRound />
          )}
          Send request
        </Button>
      </form>
    </div>
  )
}

const NewChatPanel = ({
  onBack,
  onClose,
}: {
  onBack: () => void
  onClose: () => void
}) => {
  const navigate = useNavigate()
  const friendsQuery = useFriendsQuery()
  const openConversationMutation = useOpenDirectConversationMutation()

  const handleOpenChat = (recipientUserId: string) => {
    openConversationMutation.mutate(recipientUserId, {
      onSuccess: (response) => {
        onClose()
        void navigate({
          to: "/threads/$threadId",
          params: { threadId: response.conversationId },
        })
      },
      onError: (error) => {
        toast.error(
          error instanceof ApiError ? error.message : "Could not open chat"
        )
      },
    })
  }

  return (
    <div className="p-5">
      <PanelHeader title="New Chat" onBack={onBack} />

      <div className="mt-4 max-h-80 overflow-y-auto">
        {friendsQuery.isLoading && <LoadingRow label="Loading friends" />}
        {friendsQuery.isError && (
          <EmptyState label="Could not load your friends." />
        )}
        {friendsQuery.data?.length === 0 && (
          <EmptyState label="Add a contact before starting a chat." />
        )}
        {friendsQuery.data?.map((friend) => (
          <FriendRow
            key={friend.id}
            user={friend.user}
            trailing={
              <Button
                variant="secondary"
                size="sm"
                disabled={openConversationMutation.isPending}
                onClick={() => handleOpenChat(friend.user.id)}
              >
                Chat
              </Button>
            }
          />
        ))}
      </div>
    </div>
  )
}

const PanelHeader = ({
  title,
  onBack,
}: {
  title: string
  onBack: () => void
}) => {
  return (
    <div className="flex items-center gap-3">
      <Button variant="ghost" size="icon" onClick={onBack}>
        <ArrowLeft />
      </Button>
      <h2 className="text-xl font-semibold">{title}</h2>
    </div>
  )
}

const FriendRow = ({
  user,
  trailing,
}: {
  user: FriendUser
  trailing?: ReactNode
}) => {
  const name = user.fullName || user.username

  return (
    <div className="flex min-w-0 items-center gap-3 border-t border-border/60 py-3 first:border-t-0">
      <Avatar className="size-11">
        <AvatarImage src={user.avatarUrl ?? undefined} alt={name} />
        <AvatarFallback>{getFallback(name)}</AvatarFallback>
      </Avatar>

      <div className="min-w-0 flex-1">
        <p className="truncate font-medium">{name}</p>
        <p className="truncate text-sm text-muted-foreground">{user.email}</p>
      </div>

      {trailing}
    </div>
  )
}

const LoadingRow = ({ label }: { label: string }) => {
  return (
    <div className="flex items-center gap-2 py-6 text-sm text-muted-foreground">
      <LoaderCircle className="animate-spin" />
      {label}
    </div>
  )
}

const EmptyState = ({ label }: { label: string }) => {
  return <p className="py-6 text-sm text-muted-foreground">{label}</p>
}

const getFallback = (name: string) => {
  return name
    .split(" ")
    .map((part) => part[0])
    .join("")
    .slice(0, 2)
    .toUpperCase()
}
