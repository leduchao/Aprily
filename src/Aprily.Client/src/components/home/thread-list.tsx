import { ThreadItem } from "@/components/home/thread-item"
import { threads } from "@/data/threads"
import { cn } from "@/lib/utils"
import { Link } from "@tanstack/react-router"
import { Ellipsis } from "lucide-react"

export const ThreadList = () => {
  return (
    <section className="min-h-0 flex-1 scrollbar-none overflow-x-hidden overflow-y-auto">
      <div className="sticky top-0 z-10 flex justify-between border-b border-border/60 bg-background px-4 py-2">
        <p className="text-xl font-medium">Chats</p>
        <Ellipsis />
      </div>

      <div className="bg-background shadow-sm">
        {threads.map((thread, index) => (
          <Link
            key={`${thread.name}-${index}`}
            to="/threads/$threadId"
            params={{ threadId: thread.id }}
            className={cn(
              "block",
              index > 0 && "border-t border-border/60"
            )}
          >
            <ThreadItem {...thread} />
          </Link>
        ))}
      </div>
    </section>
  )
}
