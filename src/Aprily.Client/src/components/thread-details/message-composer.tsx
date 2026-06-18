import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogTitle,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import { cn } from "@/lib/utils"
import { File as FileIcon, Mic, Plus, SendHorizontal, X } from "lucide-react"
import {
  type ChangeEvent,
  type ComponentProps,
  useEffect,
  useRef,
  useState,
} from "react"

type MessageComposerProps = {
  onSend: (message: string) => void
  disabled?: boolean
}

type SelectedAttachment = {
  id: string
  file: File
  previewUrl?: string
}

export const MessageComposer = ({
  onSend,
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
  const selectedAttachmentsRef = useRef<SelectedAttachment[]>([])
  const canSend = message.trim().length > 0 && !disabled

  useEffect(() => {
    selectedAttachmentsRef.current = selectedAttachments
  }, [selectedAttachments])

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
    const attachments = files.map((file, index) => ({
      id: `${file.name}-${file.lastModified}-${file.size}-${Date.now()}-${index}`,
      file,
      previewUrl: file.type.startsWith("image/")
        ? URL.createObjectURL(file)
        : undefined,
    }))

    setSelectedAttachments((currentAttachments) => [
      ...currentAttachments,
      ...attachments,
    ])
    event.target.value = ""
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

  const handleSubmit: ComponentProps<"form">["onSubmit"] = (event) => {
    event.preventDefault()

    if (!canSend) {
      return
    }

    onSend(message.trim())
    setMessage("")
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
          className="hidden"
          onChange={handleFileChange}
        />

        {selectedAttachments.length > 0 && (
          <div className="flex scrollbar-none gap-2 overflow-x-auto px-4 pt-3">
            {selectedAttachments.map((attachment) => {
              const isImage = Boolean(attachment.previewUrl)

              return (
                <div
                  key={attachment.id}
                  className="flex max-w-56 shrink-0 items-center gap-2 rounded-full bg-muted py-1 pr-1 pl-2 text-xs"
                >
                  <button
                    type="button"
                    className={cn(
                      "flex min-w-0 items-center gap-2",
                      isImage && "cursor-pointer"
                    )}
                    onClick={() => {
                      if (isImage) {
                        setPreviewAttachment(attachment)
                      }
                    }}
                  >
                    {attachment.previewUrl ? (
                      <img
                        src={attachment.previewUrl}
                        alt=""
                        className="size-7 shrink-0 rounded-full object-cover"
                      />
                    ) : (
                      <FileIcon className="size-4 shrink-0" />
                    )}
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
              <Plus className="size-6" />
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
