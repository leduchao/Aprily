import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogTitle,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import type { ChatMessage } from "@/lib/chat-api"
import { cn } from "@/lib/utils"
import { Image, Mic, SendHorizontal, X } from "lucide-react"
import {
  type ChangeEvent,
  type ComponentProps,
  useEffect,
  useRef,
  useState,
} from "react"
import { toast } from "sonner"

type MessageComposerProps = {
  onSend: (message: string, images: File[]) => Promise<void>
  replyingTo: ChatMessage | null
  onCancelReply: () => void
  disabled?: boolean
}

type SelectedAttachment = {
  id: string
  file: File
  previewUrl: string
}

const maxImageCount = 4
const maxImageSize = 10 * 1024 * 1024
const allowedImageTypes = ["image/jpeg", "image/png", "image/webp", "image/gif"]

export const MessageComposer = ({
  onSend,
  replyingTo,
  onCancelReply,
  disabled = false,
}: MessageComposerProps) => {
  const [message, setMessage] = useState("")
  const [isInputFocused, setIsInputFocused] = useState(false)
  const [selectedAttachments, setSelectedAttachments] = useState<
    SelectedAttachment[]
  >([])
  const [previewAttachment, setPreviewAttachment] =
    useState<SelectedAttachment | null>(null)
  const fileInputRef = useRef<HTMLInputElement>(null)
  const messageInputRef = useRef<HTMLInputElement>(null)
  const selectedAttachmentsRef = useRef<SelectedAttachment[]>([])
  const canSend =
    (message.trim().length > 0 || selectedAttachments.length > 0) && !disabled

  useEffect(() => {
    selectedAttachmentsRef.current = selectedAttachments
  }, [selectedAttachments])

  useEffect(() => {
    if (replyingTo) {
      messageInputRef.current?.focus()
    }
  }, [replyingTo])

  useEffect(() => {
    return () => {
      selectedAttachmentsRef.current.forEach((attachment) => {
        if (attachment.previewUrl) {
          URL.revokeObjectURL(attachment.previewUrl)
        }
      })
    }
  }, [])

  const handleFileChange = (event: ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(event.target.files ?? [])
    event.target.value = ""

    if (selectedAttachments.length + files.length > maxImageCount) {
      toast.error(`You can send up to ${maxImageCount} images at once`)
      return
    }

    if (files.some((file) => !allowedImageTypes.includes(file.type))) {
      toast.error("Choose JPG, PNG, WEBP, or GIF images")
      return
    }

    if (files.some((file) => file.size === 0 || file.size > maxImageSize)) {
      toast.error("Each image must be non-empty and smaller than 10 MB")
      return
    }

    const attachments = files.map((file, index) => ({
      id: `${file.name}-${file.lastModified}-${file.size}-${Date.now()}-${index}`,
      file,
      previewUrl: URL.createObjectURL(file),
    }))

    setSelectedAttachments((currentAttachments) => [
      ...currentAttachments,
      ...attachments,
    ])
  }

  const removeAttachment = (attachmentId: string) => {
    setSelectedAttachments((currentAttachments) => {
      const attachmentToRemove = currentAttachments.find(
        (attachment) => attachment.id === attachmentId
      )

      if (attachmentToRemove?.previewUrl) {
        URL.revokeObjectURL(attachmentToRemove.previewUrl)
      }

      return currentAttachments.filter(
        (attachment) => attachment.id !== attachmentId
      )
    })

    if (previewAttachment?.id === attachmentId) {
      setPreviewAttachment(null)
    }
  }

  const handleSubmit: ComponentProps<"form">["onSubmit"] = async (event) => {
    event.preventDefault()

    if (!canSend) {
      return
    }

    try {
      await onSend(
        message.trim(),
        selectedAttachments.map((attachment) => attachment.file)
      )

      selectedAttachments.forEach((attachment) => {
        URL.revokeObjectURL(attachment.previewUrl)
      })
      setMessage("")
      setSelectedAttachments([])
    } catch {
      // Keep the draft so the user can retry the failed send.
      toast.error("Could not send this message. Please try again.")
    }
  }

  return (
    <Dialog
      open={Boolean(previewAttachment)}
      onOpenChange={(isOpen) => {
        if (!isOpen) {
          setPreviewAttachment(null)
        }
      }}
    >
      <footer className="shrink-0 border-t border-border/60 bg-background">
        <input
          ref={fileInputRef}
          type="file"
          multiple
          accept="image/jpeg,image/png,image/webp,image/gif"
          className="hidden"
          onChange={handleFileChange}
        />

        {replyingTo && (
          <div className="mx-4 mt-3 flex items-center gap-3 rounded-2xl bg-muted px-3 py-2">
            <div className="min-w-0 flex-1 border-l-2 border-primary pl-3">
              <p className="text-xs font-semibold text-primary">
                Replying to{" "}
                {replyingTo.isMine ? "yourself" : replyingTo.senderUsername}
              </p>
              <p className="truncate text-xs text-muted-foreground">
                {replyingTo.content ||
                  (replyingTo.attachments.length > 0 ? "Photo" : "Message")}
              </p>
            </div>
            <Button
              type="button"
              variant="ghost"
              size="icon-sm"
              className="shrink-0 rounded-full"
              onClick={onCancelReply}
              aria-label="Cancel reply"
            >
              <X className="size-4" />
            </Button>
          </div>
        )}

        {selectedAttachments.length > 0 && (
          <div className="flex scrollbar-none gap-2 overflow-x-auto px-4 pt-3">
            {selectedAttachments.map((attachment) => {
              return (
                <div
                  key={attachment.id}
                  className="flex max-w-56 shrink-0 items-center gap-2 rounded-full bg-muted py-1 pr-1 pl-2 text-xs"
                >
                  <button
                    type="button"
                    className={cn(
                      "flex min-w-0 items-center gap-2",
                      "cursor-pointer"
                    )}
                    onClick={() => setPreviewAttachment(attachment)}
                  >
                    <img
                      src={attachment.previewUrl}
                      alt=""
                      className="size-7 shrink-0 rounded-full object-cover"
                    />
                    <span className="truncate">{attachment.file.name}</span>
                  </button>
                  <Button
                    type="button"
                    variant="ghost"
                    size="icon-xs"
                    className="rounded-full"
                    aria-label={`Remove ${attachment.file.name}`}
                    onClick={() => removeAttachment(attachment.id)}
                  >
                    <X className="size-3" />
                  </Button>
                </div>
              )
            })}
          </div>
        )}

        <form className="flex items-center px-4 py-3" onSubmit={handleSubmit}>
          <div
            className={cn(
              "shrink-0 overflow-hidden transition-all duration-300 ease-out",
              isInputFocused
                ? "mr-0 w-0 -translate-x-2 opacity-0"
                : "mr-3 w-10 translate-x-0 opacity-100"
            )}
          >
            <Button
              type="button"
              variant="ghost"
              size="icon"
              className="size-10 rounded-full"
              aria-label="Add attachment"
              tabIndex={isInputFocused ? -1 : 0}
              onClick={() => fileInputRef.current?.click()}
            >
              <Image className="size-6" />
            </Button>
          </div>

          <div
            className={cn(
              "shrink-0 overflow-hidden transition-all duration-300 ease-out",
              isInputFocused
                ? "mr-0 w-0 -translate-x-2 opacity-0"
                : "mr-3 w-10 translate-x-0 opacity-100"
            )}
          >
            <Button
              type="button"
              variant="ghost"
              size="icon"
              className="size-10 rounded-full"
              aria-label="Record voice message"
              tabIndex={isInputFocused ? -1 : 0}
            >
              <Mic className="size-6" />
            </Button>
          </div>

          <Input
            ref={messageInputRef}
            type="text"
            placeholder="Say something..."
            aria-label="Message"
            value={message}
            onChange={(event) => setMessage(event.target.value)}
            onFocus={() => setIsInputFocused(true)}
            onBlur={() => setIsInputFocused(false)}
            className="mr-3 h-12 min-w-0 flex-1 rounded-full border-transparent bg-muted px-5 text-base transition-all duration-300 ease-out focus-visible:border-primary"
          />

          <Button
            type="submit"
            variant="ghost"
            size="icon"
            className="size-10 rounded-full"
            disabled={!canSend}
            aria-label="Send message"
          >
            <SendHorizontal
              className={cn("size-6", canSend && "text-primary")}
            />
          </Button>
        </form>
      </footer>

      <DialogContent
        showCloseButton
        className="max-w-[calc(100%-2rem)] gap-3 p-3 sm:max-w-md"
      >
        <DialogTitle className="sr-only">Image preview</DialogTitle>
        <DialogDescription className="sr-only">
          Preview of the selected image attachment.
        </DialogDescription>

        {previewAttachment?.previewUrl && (
          <img
            src={previewAttachment.previewUrl}
            alt={previewAttachment.file.name}
            className="max-h-[75dvh] w-full rounded-2xl object-contain"
          />
        )}
      </DialogContent>
    </Dialog>
  )
}
