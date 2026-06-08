export type Thread = {
  id: string
  avatar: string
  name: string
  message: string
  time: string
  unreadCount?: number
  isSeen?: boolean
  isOnline?: boolean
}

export type ChatMessage = {
  id: string
  body: string
  sender: "me" | "them" | "typing"
  replyTo?: {
    name: string
    body: string
  }
}

export const threads: Thread[] = [
  {
    id: "angel-curtis",
    avatar:
      "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=160&h=160&fit=crop&crop=face",
    name: "Angel Curtis",
    message: "Please help me find a good monitor for tomorrow",
    time: "02:11",
    unreadCount: 2,
  },
  {
    id: "zaire-dorwart",
    avatar:
      "https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=160&h=160&fit=crop&crop=face",
    name: "Zaire Dorwart",
    message: "Gacor pisan kang",
    time: "02:11",
    isSeen: true,
    isOnline: true,
  },
  {
    id: "kelas-malam",
    avatar:
      "https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=160&h=160&fit=crop&crop=face",
    name: "Kelas Malam",
    message: "Bima : No one can come today?",
    time: "02:11",
    unreadCount: 2,
  },
  {
    id: "jocelyn-gouse",
    avatar:
      "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=160&h=160&fit=crop&crop=face",
    name: "Jocelyn Gouse",
    message: "You're now an admin",
    time: "02:11",
    unreadCount: 20,
    isOnline: true,
  },
]

export const threadMessages: Record<string, ChatMessage[]> = {
  "zaire-dorwart": [
    {
      id: "1",
      body: "Hi, Asal",
      sender: "them",
    },
    {
      id: "2",
      body: 'How do you buy "nice" stuff?',
      sender: "them",
    },
    {
      id: "3",
      body: "Please help me find a good monitor for the design",
      sender: "them",
    },
    {
      id: "4",
      body: "Hi, Asal",
      sender: "me",
      replyTo: {
        name: "Zaire Dorwart",
        body: "What should i call u?",
      },
    },
    {
      id: "5",
      body: "I usually buy directly to the shop to reduce the risk of damaged travel, and prevent any damage",
      sender: "me",
      replyTo: {
        name: "Zaire Dorwart",
        body: "Please help me find a good monitor for the design",
      },
    },
    {
      id: "6",
      body: "",
      sender: "typing",
    },
  ],
}

export function getThreadById(threadId: string) {
  return threads.find((thread) => thread.id === threadId)
}

export function getThreadMessages(threadId: string) {
  return threadMessages[threadId] ?? threadMessages["zaire-dorwart"]
}
