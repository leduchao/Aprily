import { ThreadItem, type ThreadItemProps } from "@/components/home/thread-item"
import { Ellipsis } from "lucide-react"

const threads: ThreadItemProps[] = [
  {
    avatar:
      "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=160&h=160&fit=crop&crop=face",
    name: "Angel Curtis",
    message: "Please help me find a good monitor for tomorrow",
    time: "02:11",
    unreadCount: 2,
  },
  {
    avatar:
      "https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=160&h=160&fit=crop&crop=face",
    name: "Zaire Dorwart",
    message: "Gacor pisan kang",
    time: "02:11",
    isSeen: true,
  },
  {
    avatar:
      "https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=160&h=160&fit=crop&crop=face",
    name: "Kelas Malam",
    message: "Bima : No one can come today?",
    time: "02:11",
    unreadCount: 2,
  },
  {
    avatar:
      "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=160&h=160&fit=crop&crop=face",
    name: "Jocelyn Gouse",
    message: "You're now an admin",
    time: "02:11",
    unreadCount: 2,
  },
  {
    avatar:
      "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=160&h=160&fit=crop&crop=face",
    name: "Angel Curtis",
    message: "Please help me find a good monitor for tomorrow",
    time: "02:11",
    unreadCount: 2,
  },
  {
    avatar:
      "https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=160&h=160&fit=crop&crop=face",
    name: "Zaire Dorwart",
    message: "Gacor pisan kang",
    time: "02:11",
    isSeen: true,
  },
  {
    avatar:
      "https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=160&h=160&fit=crop&crop=face",
    name: "Kelas Malam",
    message: "Bima : No one can come today?",
    time: "02:11",
    unreadCount: 2,
  },
  {
    avatar:
      "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=160&h=160&fit=crop&crop=face",
    name: "Jocelyn Gouse",
    message: "You're now an admin",
    time: "02:11",
    unreadCount: 2,
  },
  {
    avatar:
      "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=160&h=160&fit=crop&crop=face",
    name: "Angel Curtis",
    message: "Please help me find a good monitor for tomorrow",
    time: "02:11",
    unreadCount: 2,
  },
  {
    avatar:
      "https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=160&h=160&fit=crop&crop=face",
    name: "Zaire Dorwart",
    message: "Gacor pisan kang",
    time: "02:11",
    isSeen: true,
  },
  {
    avatar:
      "https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=160&h=160&fit=crop&crop=face",
    name: "Kelas Malam",
    message: "Bima : No one can come today?",
    time: "02:11",
    unreadCount: 2,
  },
  {
    avatar:
      "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=160&h=160&fit=crop&crop=face",
    name: "Jocelyn Gouse",
    message: "You're now an admin",
    time: "02:11",
    unreadCount: 20,
  },
]

export const ThreadList = () => {
  return (
    <section className="min-h-0 flex-1 scrollbar-none overflow-x-hidden overflow-y-auto">
      <div className="sticky top-0 z-10 flex justify-between border-b border-border/60 bg-background px-4 py-2">
        <p className="text-xl font-medium">Chats</p>
        <Ellipsis />
      </div>

      <div className="bg-background shadow-sm">
        {threads.map((thread, index) => (
          <div
            key={`${thread.name}-${index}`}
            className={index > 0 ? "border-t border-border/60" : undefined}
          >
            <ThreadItem {...thread} />
          </div>
        ))}
      </div>
    </section>
  )
}
