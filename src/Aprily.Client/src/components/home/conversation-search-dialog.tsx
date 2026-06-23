import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar"
import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import { useSearchConversationsQuery } from "@/lib/chat-api"
import { useNavigate } from "@tanstack/react-router"
import { LoaderCircle, Search } from "lucide-react"
import { useEffect, useState } from "react"

const searchDebounceMs = 400

export const ConversationSearchDialog = () => {
  const navigate = useNavigate()
  const [isOpen, setIsOpen] = useState(false)
  const [query, setQuery] = useState("")
  const [debouncedQuery, setDebouncedQuery] = useState("")
  const normalizedQuery = query.trim()
  const searchQuery = useSearchConversationsQuery(debouncedQuery)

  useEffect(() => {
    const timeout = window.setTimeout(() => {
      setDebouncedQuery(normalizedQuery)
    }, searchDebounceMs)

    return () => window.clearTimeout(timeout)
  }, [normalizedQuery])

  const handleOpenChange = (open: boolean) => {
    setIsOpen(open)

    if (!open) {
      setQuery("")
      setDebouncedQuery("")
    }
  }

  const openConversation = (conversationId: string) => {
    setIsOpen(false)
    void navigate({
      to: "/threads/$threadId",
      params: { threadId: conversationId },
    })
  }

  const isWaitingForDebounce = normalizedQuery !== debouncedQuery

  return (
    <Dialog open={isOpen} onOpenChange={handleOpenChange}>
      <DialogTrigger asChild>
        <Button
          type="button"
          variant="ghost"
          size="icon"
          className="rounded-full"
          aria-label="Search conversations"
        >
          <Search className="size-5" />
        </Button>
      </DialogTrigger>

      <DialogContent className="max-w-[calc(100%-2rem)] gap-4 sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Search conversations</DialogTitle>
          <DialogDescription>
            Search by a person or group name.
          </DialogDescription>
        </DialogHeader>

        <div className="relative">
          <Input
            autoFocus
            value={query}
            maxLength={100}
            onChange={(event) => setQuery(event.target.value)}
            placeholder="Search conversations..."
            className="h-12 rounded-full px-4"
          />
        </div>

        <div className="max-h-[55dvh] min-h-32 overflow-y-auto">
          {!normalizedQuery && (
            <p className="py-10 text-center text-sm text-muted-foreground">
              Enter a name to find a conversation.
            </p>
          )}

          {normalizedQuery &&
            (isWaitingForDebounce || searchQuery.isLoading) && (
              <div className="flex items-center justify-center gap-2 py-10 text-sm text-muted-foreground">
                <LoaderCircle className="size-4 animate-spin" />
                Searching
              </div>
            )}

          {!isWaitingForDebounce && searchQuery.isError && (
            <p className="py-10 text-center text-sm text-muted-foreground">
              Could not search conversations.
            </p>
          )}

          {!isWaitingForDebounce &&
            searchQuery.isSuccess &&
            searchQuery.data.length === 0 && (
              <p className="py-10 text-center text-sm text-muted-foreground">
                No matching conversations.
              </p>
            )}

          {!isWaitingForDebounce &&
            searchQuery.data?.map((conversation) => {
              const name =
                conversation.otherUser.fullName ||
                conversation.otherUser.username

              return (
                <button
                  key={conversation.id}
                  type="button"
                  className="flex w-full items-center gap-3 border-t border-border/60 px-2 py-3 text-left first:border-t-0 hover:bg-muted/60"
                  onClick={() => openConversation(conversation.id)}
                >
                  <Avatar className="size-11">
                    <AvatarImage
                      src={conversation.otherUser.avatarUrl ?? undefined}
                      alt={name}
                    />
                    <AvatarFallback>{getFallback(name)}</AvatarFallback>
                  </Avatar>

                  <div className="min-w-0 flex-1">
                    <p className="truncate font-medium">{name}</p>
                    <p className="truncate text-sm text-muted-foreground">
                      @{conversation.otherUser.username}
                    </p>
                  </div>
                </button>
              )
            })}
        </div>
      </DialogContent>
    </Dialog>
  )
}

const getFallback = (name: string) =>
  name
    .split(" ")
    .map((part) => part[0])
    .join("")
    .slice(0, 2)
    .toUpperCase()
