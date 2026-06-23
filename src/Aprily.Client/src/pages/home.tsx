import { Footer } from "@/components/home/footer"
import { ConversationSearchDialog } from "@/components/home/conversation-search-dialog"
import { FriendRequestsDialog } from "@/components/home/friend-requests-dialog"
import { NoteList } from "@/components/home/note-list"
import { ThreadList } from "@/components/home/thread-list"

export const HomePage = () => {
  return (
    <main className="flex h-dvh w-dvw max-w-full flex-col overflow-hidden">
      {/* app name and search icon */}
      <div className="flex shrink-0 items-center justify-between p-4">
        <h1 className="text-3xl font-bold">Aprily</h1>
        <div className="flex items-center gap-1">
          <FriendRequestsDialog />
          <ConversationSearchDialog />
        </div>
      </div>

      {/* list friend notes */}
      <NoteList />

      {/* list chats */}
      <ThreadList />

      {/* footer */}
      <Footer />
    </main>
  )
}
