import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogTitle,
} from "@/components/ui/dialog"
import type { ChatMessage, ChatMessageAttachment } from "@/lib/chat-api"
import { cn } from "@/lib/utils"
import { useState } from "react"

type MessageBubbleProps = {
  message: ChatMessage
}

export const MessageBubble = ({ message }: MessageBubbleProps) => {
  const [previewAttachment, setPreviewAttachment] =
    useState<ChatMessageAttachment | null>(null)

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
        className={cn("flex", message.isMine ? "justify-end" : "justify-start")}
      >
        <div
          className={cn(
            "max-w-[78%] overflow-hidden rounded-2xl p-1 leading-relaxed",
            message.isMine
              ? "rounded-br-sm bg-primary text-gray-900"
              : "rounded-bl-sm bg-muted text-foreground"
          )}
        >
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

          {message.content && <p className="px-2 py-1">{message.content}</p>}
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
