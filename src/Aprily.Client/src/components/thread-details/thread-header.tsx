import {
  Avatar,
  AvatarBadge,
  AvatarFallback,
  AvatarImage,
} from "@/components/ui/avatar"
import { Button } from "@/components/ui/button"
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover"
import { Link } from "@tanstack/react-router"
import {
  ArrowLeft,
  EllipsisVertical,
  Phone,
  UserRound,
  Video,
} from "lucide-react"

const threadActions = [
  {
    icon: Video,
    label: "Video call",
  },
  {
    icon: Phone,
    label: "Voice call",
  },
  {
    icon: UserRound,
    label: "View profile",
  },
]

export type ThreadHeaderInfo = {
  avatarUrl: string | null
  name: string
  isOnline?: boolean
}

type ThreadHeaderProps = {
  thread: ThreadHeaderInfo
}

export const ThreadHeader = ({ thread }: ThreadHeaderProps) => {
  const fallback = thread.name
    .split(" ")
    .map((part) => part[0])
    .join("")
    .slice(0, 2)

  return (
    <header className="flex shrink-0 items-center gap-3 border-b border-border/60 px-4 py-4">
      <Button
        variant="ghost"
        size="icon"
        className="size-10 rounded-full"
        asChild
      >
        <Link to="/">
          <ArrowLeft className="size-6" />
        </Link>
      </Button>

      <Avatar className="size-11">
        <AvatarImage src={thread.avatarUrl ?? undefined} alt={thread.name} />
        <AvatarFallback>{fallback}</AvatarFallback>
        {thread.isOnline && (
          <AvatarBadge className="bg-green-600 dark:bg-green-500" />
        )}
      </Avatar>

      <div className="min-w-0 flex-1">
        <p className="truncate leading-tight font-semibold">{thread.name}</p>
        <p className="text-xs text-muted-foreground">
          {thread.isOnline ? "Online" : "Online 5 minutes ago"}
        </p>
      </div>

      <Popover>
        <PopoverTrigger asChild>
          <Button
            variant="ghost"
            size="icon"
            className="size-10 rounded-full"
            aria-label="Thread actions"
          >
            <EllipsisVertical className="size-6" />
          </Button>
        </PopoverTrigger>

        <PopoverContent
          align="end"
          sideOffset={8}
          className="w-56 gap-1 rounded-2xl p-2"
        >
          {threadActions.map((action) => {
            const Icon = action.icon

            return (
              <Button
                key={action.label}
                variant="ghost"
                className="h-11 w-full justify-start gap-3 rounded-xl px-3"
              >
                <Icon className="size-5" />
                <span className="">{action.label}</span>
              </Button>
            )
          })}
        </PopoverContent>
      </Popover>
    </header>
  )
}
