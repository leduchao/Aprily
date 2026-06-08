import { useState } from "react"
import {
  ContactRound,
  Home,
  MessageSquare,
  Plus,
  UserRound,
  Users,
} from "lucide-react"

import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { cn } from "@/lib/utils"

const actions = [
  {
    icon: MessageSquare,
    title: "New Chat",
    description: "Send a message to your contact",
  },
  {
    icon: ContactRound,
    title: "New Contact",
    description: "Add a contact to be able to send messages",
  },
  {
    icon: Users,
    title: "New Community",
    description: "Join the community around you",
  },
]

export const Footer = () => {
  const [isOpen, setIsOpen] = useState(false)

  return (
    <Dialog open={isOpen} onOpenChange={setIsOpen}>
      <footer className="shrink-0 border-t border-border/60 bg-background py-4">
        <div className="grid grid-cols-[1fr_auto_1fr] items-center">
          <div className="flex justify-center">
            <Button
              variant="ghost"
              size="icon"
              className="size-12 rounded-full text-foreground"
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
              className="size-12 rounded-full text-muted-foreground"
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
          Choose whether to start a chat, add a contact, or create a community.
        </DialogDescription>

        <div className="overflow-hidden rounded-3xl bg-card shadow-2xl">
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
