import { CheckCheck } from "lucide-react"

import {
  Avatar,
  AvatarBadge,
  AvatarFallback,
  AvatarImage,
} from "@/components/ui/avatar"

export type ThreadItemProps = {
  id: string
  avatar: string
  name: string
  message: string
  time: string
  unreadCount?: number
  isSeen?: boolean
  isOnline?: boolean
}

export const ThreadItem = ({
  avatar,
  name,
  message,
  time,
  unreadCount,
  isSeen = false,
  isOnline = false,
}: ThreadItemProps) => {
  const fallback = name
    .split(" ")
    .map((part) => part[0])
    .join("")
    .slice(0, 2)

  return (
    <div className="flex min-w-0 items-center gap-4 px-4 py-3">
      <Avatar className="size-15">
        <AvatarImage src={avatar} alt={name} />
        <AvatarFallback>{fallback}</AvatarFallback>
        {isOnline && <AvatarBadge className="bg-green-600 dark:bg-green-500" />}
      </Avatar>

      <div className="min-w-0 flex-1">
        <p className="truncate text-lg leading-tight font-semibold text-foreground">
          {name}
        </p>
        <div className="mt-1 flex min-w-0 items-center gap-1 text-sm">
          {isSeen && (
            <CheckCheck className="size-4 shrink-0 text-muted-foreground" />
          )}
          <p className="truncate text-foreground/80">{message}</p>
        </div>
      </div>

      <div className="flex shrink-0 flex-col items-end gap-3">
        <span className="text-sm text-muted-foreground">{time}</span>
        {unreadCount ? (
          <span className="flex size-6 items-center justify-center rounded-full bg-primary text-xs font-semibold text-gray-900">
            {unreadCount}
          </span>
        ) : (
          <span className="size-6" />
        )}
      </div>
    </div>
  )
}
