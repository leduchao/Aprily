import { Plus } from "lucide-react"
import { Avatar, AvatarBadge, AvatarFallback, AvatarImage } from "../ui/avatar"
import { Button } from "../ui/button"

const notes = [
  {
    avatar: "https://github.com/shadcn.png",
    name: "Hao Le",
    note: "Hôm nay là thứ mấy nhỉ",
    status: "active",
  },
  {
    avatar: "https://github.com/shadcn.png",
    name: "Thao Nguyen",
    note: "hello world",
    status: "inactive",
  },
  {
    avatar: "https://github.com/shadcn.png",
    name: "Taylor",
    note: "",
    status: "active",
  },
  {
    avatar: "https://github.com/shadcn.png",
    name: "Bruno",
    note: "hello world",
    status: "active",
  },
  {
    avatar: "https://github.com/shadcn.png",
    name: "Hao Le",
    note: "hello world",
    status: "active",
  },
  {
    avatar: "https://github.com/shadcn.png",
    name: "Thao Nguyen",
    note: "hello world",
    status: "inactive",
  },
  {
    avatar: "https://github.com/shadcn.png",
    name: "Taylor",
    note: "hello world",
    status: "active",
  },
  {
    avatar: "https://github.com/shadcn.png",
    name: "Bruno",
    note: "hello world",
    status: "active",
  },
  {
    avatar: "https://github.com/shadcn.png",
    name: "Hao Le",
    note: "hello world",
    status: "active",
  },
  {
    avatar: "https://github.com/shadcn.png",
    name: "Thao Nguyen",
    note: "hello world",
    status: "inactive",
  },
  {
    avatar: "https://github.com/shadcn.png",
    name: "Taylor",
    note: "hello world",
    status: "active",
  },
  {
    avatar: "https://github.com/shadcn.png",
    name: "Bruno",
    note: "hello world",
    status: "active",
  },
]

export const NoteList = () => {
  return (
    <section className="shrink-0">
      <div className="flex scrollbar-none gap-10 overflow-x-auto px-4 py-4">
        {/* add note */}
        <Button
          variant="ghost"
          className="flex h-24 w-16 shrink-0 flex-col items-center justify-end gap-0 rounded-none p-0 hover:bg-transparent"
        >
          <span className="relative">
            <span className="absolute bottom-12 left-1/2 z-10 w-20 -translate-x-1/2 rounded-2xl bg-primary px-2.5 py-1 text-center text-[11px] leading-tight break-words text-card-foreground shadow-sm">
              <span className="line-clamp-2 text-background">Your note...</span>
            </span>

            <span className="flex size-16 items-center justify-center rounded-full border-2 border-dashed border-muted-foreground">
              <Plus className="size-8" />
            </span>
          </span>
          <span className="mt-2 w-full truncate text-center text-sm">
            Add note
          </span>
        </Button>

        {/* list note */}
        {notes.map((item, index) => (
          <div
            key={item.name + index}
            className="flex h-24 w-16 shrink-0 flex-col items-center justify-end"
          >
            <div className="relative">
              {item.note?.trim() && (
                <p className="absolute bottom-12 left-1/2 z-10 w-20 -translate-x-1/2 rounded-2xl bg-primary px-2.5 py-1 text-center text-[11px] leading-tight break-words text-card-foreground shadow-sm">
                  <span className="line-clamp-2 text-background">
                    {item.note}
                  </span>
                </p>
              )}

              <Avatar className="size-16">
                <AvatarImage src={item.avatar} alt="@shadcn" />
                <AvatarFallback>CN</AvatarFallback>
                {item.status === "active" && (
                  <AvatarBadge className="bg-green-600 dark:bg-green-800" />
                )}
              </Avatar>
            </div>
            <p className="mt-2 w-full truncate text-center text-sm">
              {item.name}
            </p>
          </div>
        ))}
      </div>
    </section>
  )
}
