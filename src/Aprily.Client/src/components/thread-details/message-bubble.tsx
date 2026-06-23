import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogTitle,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover"
import type {
  ChatMessage,
  ChatMessageAttachment,
  MessageReactionType,
} from "@/lib/chat-api"
import { cn } from "@/lib/utils"
import { ArrowLeft, EllipsisVertical, Reply, SmilePlus } from "lucide-react"
import { useState } from "react"

type MessageBubbleProps = {
  message: ChatMessage
  onReply: () => void
  onReact: (type: MessageReactionType) => void
  onNavigateToMessage: (messageId: string) => void
  isHighlighted?: boolean
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
}: MessageBubbleProps) => {
  const [previewAttachment, setPreviewAttachment] =
    useState<ChatMessageAttachment | null>(null)
  const [isActionMenuOpen, setIsActionMenuOpen] = useState(false)
  const [isShowingReactions, setIsShowingReactions] = useState(false)

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
        className={cn("flex", message.isMine ? "justify-end" : "justify-start")}
      >
        <div
          className={cn(
            "group relative flex max-w-[78%] flex-col rounded-2xl transition-shadow duration-300",
            isHighlighted &&
              "ring-2 ring-primary ring-offset-2 ring-offset-background",
            message.isMine ? "items-end" : "items-start"
          )}
        >
          <div
            className={cn(
              "overflow-hidden rounded-2xl p-1 leading-relaxed",
              message.isMine
                ? "rounded-br-sm bg-primary text-gray-900"
                : "rounded-bl-sm bg-muted text-foreground"
            )}
          >
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
          </div>

          {message.reactions.length > 0 && (
            <div className="mt-1 flex flex-wrap gap-1">
              {message.reactions.map((reaction) => {
                const option = reactionOptions.find(
                  (candidate) => candidate.type === reaction.type
                )

                return (
                  <button
                    key={reaction.type}
                    type="button"
                    className={cn(
                      "flex h-7 items-center gap-1 rounded-full border bg-background px-2 text-xs shadow-sm",
                      reaction.reactedByMe && "border-primary bg-primary/10"
                    )}
                    onClick={() => onReact(reaction.type)}
                    aria-label={`${option?.label || reaction.type}: ${reaction.count}`}
                  >
                    <span>{option?.emoji}</span>
                    <span>{reaction.count}</span>
                  </button>
                )
              })}
            </div>
          )}

          <div
            className={cn(
              "pointer-events-none absolute top-1/2 flex -translate-y-1/2 items-center opacity-0 transition-opacity group-focus-within:pointer-events-auto group-focus-within:opacity-70 group-hover:pointer-events-auto group-hover:opacity-70",
              message.isMine ? "right-full pr-1" : "left-full pl-1",
              isActionMenuOpen && "pointer-events-auto opacity-70"
            )}
          >
            <Popover
              open={isActionMenuOpen}
              onOpenChange={(open) => {
                setIsActionMenuOpen(open)

                if (!open) {
                  setIsShowingReactions(false)
                }
              }}
            >
              <PopoverTrigger asChild>
                <Button
                  type="button"
                  variant="ghost"
                  size="icon-xs"
                  className="rounded-full"
                  aria-label="Message actions"
                >
                  <EllipsisVertical className="size-4" />
                </Button>
              </PopoverTrigger>
              <PopoverContent
                side="top"
                align={message.isMine ? "end" : "start"}
                className={cn(
                  "gap-1 p-1.5",
                  isShowingReactions ? "w-auto rounded-full" : "w-40 rounded-xl"
                )}
              >
                {isShowingReactions ? (
                  <div className="flex items-center gap-1">
                    <Button
                      type="button"
                      variant="ghost"
                      size="icon-sm"
                      className="shrink-0 rounded-full"
                      onClick={() => setIsShowingReactions(false)}
                      aria-label="Back to message actions"
                    >
                      <ArrowLeft className="size-4" />
                    </Button>

                    {reactionOptions.map((option) => (
                      <button
                        key={option.type}
                        type="button"
                        className="flex size-9 items-center justify-center rounded-full text-xl transition-transform hover:scale-125 hover:bg-muted"
                        onClick={() => {
                          onReact(option.type)
                          setIsActionMenuOpen(false)
                          setIsShowingReactions(false)
                        }}
                        aria-label={option.label}
                        title={option.label}
                      >
                        {option.emoji}
                      </button>
                    ))}
                  </div>
                ) : (
                  <div className="flex flex-col gap-1">
                    <button
                      type="button"
                      className="flex items-center gap-2 rounded-lg px-3 py-2 text-left transition-colors hover:bg-muted"
                      onClick={() => {
                        setIsActionMenuOpen(false)
                        onReply()
                      }}
                    >
                      <Reply className="size-4" />
                      <span>Reply</span>
                    </button>
                    <button
                      type="button"
                      className="flex items-center gap-2 rounded-lg px-3 py-2 text-left transition-colors hover:bg-muted"
                      onClick={() => setIsShowingReactions(true)}
                    >
                      <SmilePlus className="size-4" />
                      <span>React</span>
                    </button>
                  </div>
                )}
              </PopoverContent>
            </Popover>
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
