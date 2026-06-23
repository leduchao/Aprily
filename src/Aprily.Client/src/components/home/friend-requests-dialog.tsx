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
import { ApiError } from "@/lib/api-client"
import {
  type FriendRequest,
  useAcceptFriendRequestMutation,
  useDeclineFriendRequestMutation,
  useIncomingFriendRequestsQuery,
} from "@/lib/friends-api"
import { Check, LoaderCircle, UserRoundPlus, X } from "lucide-react"
import { toast } from "sonner"

export const FriendRequestsDialog = () => {
  const requestsQuery = useIncomingFriendRequestsQuery()
  const acceptMutation = useAcceptFriendRequestMutation()
  const declineMutation = useDeclineFriendRequestMutation()
  const requestCount = requestsQuery.data?.length ?? 0
  const isResponding = acceptMutation.isPending || declineMutation.isPending

  const handleAccept = (request: FriendRequest) => {
    acceptMutation.mutate(request.id, {
      onSuccess: () => toast.success("Friend request accepted"),
      onError: (error) => {
        toast.error(
          error instanceof ApiError
            ? error.message
            : "Could not accept friend request"
        )
      },
    })
  }

  const handleDecline = (request: FriendRequest) => {
    declineMutation.mutate(request.id, {
      onSuccess: () => toast.success("Friend request declined"),
      onError: (error) => {
        toast.error(
          error instanceof ApiError
            ? error.message
            : "Could not decline friend request"
        )
      },
    })
  }

  return (
    <Dialog>
      <DialogTrigger asChild>
        <Button
          type="button"
          variant="ghost"
          size="icon"
          className="relative rounded-full"
          aria-label={
            requestCount > 0
              ? `${requestCount} pending friend requests`
              : "Friend requests"
          }
        >
          <UserRoundPlus className="size-5" />
          {requestCount > 0 && (
            <span className="absolute -top-1 -right-1 flex min-w-5 items-center justify-center rounded-full bg-destructive px-1 text-[10px] leading-5 font-bold text-white shadow-sm">
              {requestCount > 99 ? "99+" : requestCount}
            </span>
          )}
        </Button>
      </DialogTrigger>

      <DialogContent className="max-w-[calc(100%-2rem)] sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Friend requests</DialogTitle>
          <DialogDescription>
            Review people who want to connect with you.
          </DialogDescription>
        </DialogHeader>

        <div className="max-h-[60dvh] overflow-y-auto">
          {requestsQuery.isLoading && (
            <div className="flex items-center justify-center gap-2 py-8 text-sm text-muted-foreground">
              <LoaderCircle className="size-4 animate-spin" />
              Loading requests
            </div>
          )}

          {requestsQuery.isError && (
            <p className="py-8 text-center text-sm text-muted-foreground">
              Could not load friend requests.
            </p>
          )}

          {!requestsQuery.isLoading && requestCount === 0 && (
            <p className="py-8 text-center text-sm text-muted-foreground">
              No pending friend requests.
            </p>
          )}

          {requestsQuery.data?.map((request) => {
            const name =
              request.requester.fullName || request.requester.username

            return (
              <div
                key={request.id}
                className="flex min-w-0 items-center gap-3 border-t border-border/60 py-3 first:border-t-0"
              >
                <Avatar className="size-11">
                  <AvatarImage
                    src={request.requester.avatarUrl ?? undefined}
                    alt={name}
                  />
                  <AvatarFallback>{getFallback(name)}</AvatarFallback>
                </Avatar>

                <div className="min-w-0 flex-1">
                  <p className="truncate font-medium">{name}</p>
                  <p className="truncate text-sm text-muted-foreground">
                    @{request.requester.username}
                  </p>
                </div>

                <div className="flex gap-1">
                  <Button
                    type="button"
                    variant="secondary"
                    size="icon-sm"
                    className="rounded-full"
                    disabled={isResponding}
                    onClick={() => handleAccept(request)}
                    aria-label={`Accept request from ${name}`}
                  >
                    <Check />
                  </Button>
                  <Button
                    type="button"
                    variant="destructive"
                    size="icon-sm"
                    className="rounded-full"
                    disabled={isResponding}
                    onClick={() => handleDecline(request)}
                    aria-label={`Decline request from ${name}`}
                  >
                    <X />
                  </Button>
                </div>
              </div>
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
