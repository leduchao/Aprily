import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogTitle,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar"
import type {
  ChatMessage,
  ChatMessageAttachment,
  MessageReactionType,
} from "@/lib/chat-api"
import { cn } from "@/lib/utils"
import { Reply, SmilePlus } from "lucide-react"
import { useState } from "react"

type MessageBubbleProps = {
  message: ChatMessage
  onReply: () => void
  onReact: (type: MessageReactionType) => void
  onNavigateToMessage: (messageId: string) => void
  isHighlighted?: boolean
  showSenderName?: boolean
  showSenderAvatar?: boolean
  showReactionDetails?: boolean
  showTimestamp?: boolean
}

const reactionOptions: Array<{
  type: MessageReactionType
  emoji: string
  label: string
}> = [
  { type: "like", emoji: "👍", label: "Like" },
  { type: "love", emoji: "❤️", label: "Love" },
  { type: "haha", emoji: "😂", label: "Haha" },
  { type: "sad", emoji: "😢", label: "Sad" },
  { type: "wow", emoji: "😮", label: "Wow" },
  { type: "angry", emoji: "😡", label: "Angry" },
]

export const MessageBubble = ({
  message,
  onReply,
  onReact,
  onNavigateToMessage,
  isHighlighted = false,
  showSenderName = false,
  showSenderAvatar = false,
  showReactionDetails = false,
  showTimestamp = true,
}: MessageBubbleProps) => {
  const [previewAttachment, setPreviewAttachment] =
    useState<ChatMessageAttachment | null>(null)
  const [isQuickReactionOpen, setIsQuickReactionOpen] = useState(false)

  return (
    <Dialog
      open={previewAttachment !== null}
      onOpenChange={(open) => {
        if (!open) {
          setPreviewAttachment(null)
        }
      }}
    >
      <div
        id={`chat-message-${message.id}`}
        className={cn(message.reactions.length > 0 && "pb-5")}
      >
        <div
          className={cn(
            "group/message-row flex items-end gap-2",
            message.isMine ? "justify-end" : "justify-start"
          )}
        >
          {showSenderAvatar && (
            <Avatar className="size-8 shrink-0 self-start">
              <AvatarImage
                src={message.senderAvatarUrl ?? undefined}
                alt={message.senderUsername}
              />
              <AvatarFallback className="text-[10px]">
                {getFallback(message.senderUsername)}
              </AvatarFallback>
            </Avatar>
          )}

          <div
            className={cn(
              "group relative flex max-w-[78%] flex-col",
              message.isMine ? "items-end" : "items-start"
            )}
          >
            <div
              className={cn(
                "overflow-hidden rounded-2xl p-1 leading-relaxed transition-shadow duration-300",
                message.isMine
                  ? "rounded-br-sm bg-primary text-gray-900"
                  : "rounded-bl-sm bg-muted text-foreground",
                isHighlighted &&
                  "ring-2 ring-primary ring-offset-2 ring-offset-background"
              )}
            >
              {showSenderName && (
                <p className="px-2 pt-1 pb-0.5 text-xs font-semibold text-primary">
                  {message.senderUsername}
                </p>
              )}

              {message.replyTo && (
                <button
                  type="button"
                  className="mx-2 mt-2 mb-1 block min-w-0 cursor-pointer border-l-2 border-current px-2 text-left text-xs leading-4 transition-opacity hover:opacity-75"
                  onClick={() => onNavigateToMessage(message.replyTo!.id)}
                  aria-label="Go to original message"
                >
                  <p className="truncate font-semibold">
                    {message.replyTo.senderUsername}
                  </p>
                  <p className="max-w-72 truncate opacity-70">
                    {message.replyTo.content ||
                      (message.replyTo.hasAttachments ? "Photo" : "Message")}
                  </p>
                </button>
              )}

              {message.attachments.length > 0 && (
                <div
                  className={cn(
                    "grid gap-1",
                    message.attachments.length > 1 && "grid-cols-2"
                  )}
                >
                  {message.attachments.map((attachment) => (
                    <button
                      key={attachment.id}
                      type="button"
                      className="block cursor-zoom-in overflow-hidden rounded-xl"
                      onClick={() => setPreviewAttachment(attachment)}
                      aria-label={`Preview ${attachment.originalFileName || "chat image"}`}
                    >
                      <img
                        src={attachment.url}
                        alt={attachment.originalFileName || "Chat image"}
                        loading="lazy"
                        className="max-h-72 w-full object-cover"
                      />
                    </button>
                  ))}
                </div>
              )}

              {message.content && (
                <p className={cn("px-2 py-1", message.replyTo && "pt-2")}>
                  {message.content}
                </p>
              )}

              {showTimestamp && (
                <p className="px-2 pb-1 text-right text-[10px] leading-none opacity-60">
                  {formatMessageTime(message.sentAt)}
                </p>
              )}
            </div>

            {message.reactions.length > 0 && (
              <div
                className={cn(
                  "absolute top-full z-10 flex w-max max-w-[min(18rem,80vw)] flex-wrap gap-1 transition-transform",
                  message.isMine ? "right-0" : "left-0",
                  isHighlighted ? "translate-y-1" : "-translate-y-1"
                )}
              >
                {message.reactions.map((reaction) => {
                  const option = reactionOptions.find(
                    (candidate) => candidate.type === reaction.type
                  )

                  return (
                    <button
                      key={reaction.type}
                      type="button"
                      className={cn(
                        "group/reaction relative flex h-7 items-center gap-1 rounded-full border bg-background px-2 text-xs shadow-sm",
                        reaction.reactedByMe && "border-primary bg-primary/10"
                      )}
                      aria-label={`${option?.label || reaction.type}: ${(reaction.reactedBy ?? []).join(", ")}`}
                      onClick={() => onReact(reaction.type)}
                    >
                      <span>{option?.emoji}</span>
                      <span>{reaction.count}</span>
                      {showReactionDetails &&
                        reaction.reactedBy?.length > 0 && (
                          <span className="pointer-events-none absolute bottom-full left-0 z-20 mb-2 max-w-64 min-w-max rounded-lg bg-foreground px-2.5 py-1.5 text-left text-xs font-medium whitespace-normal text-background opacity-0 shadow-lg transition-opacity group-hover/reaction:opacity-100 group-focus-visible/reaction:opacity-100">
                            {reaction.reactedBy.join(", ")}
                          </span>
                        )}
                    </button>
                  )
                })}
              </div>
            )}

            <div
              className={cn(
                "pointer-events-none absolute top-1/2 flex -translate-y-1/2 items-center opacity-0 transition-opacity group-focus-within/message-row:pointer-events-auto group-focus-within/message-row:opacity-70 group-hover/message-row:pointer-events-auto group-hover/message-row:opacity-70",
                message.isMine ? "right-full pr-1" : "left-full pl-1"
              )}
            >
              <Button
                type="button"
                variant="ghost"
                size="icon-xs"
                className="rounded-full"
                onClick={onReply}
                aria-label="Reply to message"
              >
                <Reply className="size-4" />
              </Button>

              <div
                className="relative"
                onMouseEnter={() => setIsQuickReactionOpen(true)}
                onMouseLeave={() => setIsQuickReactionOpen(false)}
                onFocus={() => setIsQuickReactionOpen(true)}
                onBlur={(event) => {
                  if (!event.currentTarget.contains(event.relatedTarget)) {
                    setIsQuickReactionOpen(false)
                  }
                }}
              >
                <Button
                  type="button"
                  variant="ghost"
                  size="icon-xs"
                  className="rounded-full"
                  aria-label="React to message"
                  onClick={() => setIsQuickReactionOpen(true)}
                >
                  <SmilePlus className="size-4" />
                </Button>

                {isQuickReactionOpen && (
                  <div
                    className={cn(
                      "absolute bottom-full z-50 flex gap-1 rounded-full bg-popover p-1.5 shadow-lg ring-1 ring-foreground/5",
                      message.isMine ? "right-0" : "left-0"
                    )}
                  >
                    {reactionOptions.map((option) => (
                      <button
                        key={option.type}
                        type="button"
                        className="flex size-9 items-center justify-center rounded-full text-xl transition-transform hover:scale-125 hover:bg-muted"
                        onClick={() => {
                          onReact(option.type)
                          setIsQuickReactionOpen(false)
                        }}
                        aria-label={option.label}
                        title={option.label}
                      >
                        {option.emoji}
                      </button>
                    ))}
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>

      <DialogContent className="max-w-[calc(100%-2rem)] gap-3 p-3 sm:max-w-3xl">
        <DialogTitle className="sr-only">Image preview</DialogTitle>
        <DialogDescription className="sr-only">
          Full-size preview of the selected chat image.
        </DialogDescription>

        {previewAttachment && (
          <img
            src={previewAttachment.url}
            alt={previewAttachment.originalFileName || "Chat image"}
            className="max-h-[82dvh] w-full rounded-2xl object-contain"
          />
        )}
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

const formatMessageTime = (sentAt: string) =>
  new Intl.DateTimeFormat(undefined, {
    hour: "2-digit",
    minute: "2-digit",
  }).format(new Date(sentAt))
