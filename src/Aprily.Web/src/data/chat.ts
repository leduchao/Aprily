export type Thread = {
  id: string;
  title: string;
  subtitle: string;
  time?: string;
  lastMessage: string;
  unread: number;
};

export type Message = {
  from: "me" | "them";
  text: string;
};

export const chatThreads: Thread[] = [
  {
    id: "friend-1",
    title: "Mai",
    subtitle: "Đã gửi 2 ảnh",
    time: "Last week",
    lastMessage: "Ok, mình sẽ qua chơi lúc 7h",
    unread: 1,
  },
  {
    id: "group-1",
    title: "Team Product",
    subtitle: "Đã thêm tài liệu design",
    time: "April 5",
    lastMessage: "Mọi người có ý kiến gì không?",
    unread: 3,
  },
  {
    id: "friend-2",
    title: "Hùng",
    subtitle: "Đang online",
    lastMessage: "Mai nhé, tớ bận họp xong sẽ gọi lại",
    unread: 0,
  },
  {
    id: "friend-1",
    title: "Mai",
    subtitle: "Đã gửi 2 ảnh",
    lastMessage: "Ok, mình sẽ qua chơi lúc 7h",
    unread: 1,
  },
  {
    id: "group-1",
    title: "Team Product",
    subtitle: "Đã thêm tài liệu design",
    lastMessage: "Mọi người có ý kiến gì không?",
    unread: 3,
  },
  {
    id: "friend-2",
    title: "Hùng",
    subtitle: "Đang online",
    lastMessage:
      "Mai nhé, tớ bận họp xong sẽ gọi lại. Mai nhé, tớ bận họp xong sẽ gọi lại. Mai nhé, tớ bận họp xong sẽ gọi lại",
    unread: 0,
  },
  {
    id: "friend-1",
    title: "Mai",
    subtitle: "Đã gửi 2 ảnh",
    lastMessage: "Ok, mình sẽ qua chơi lúc 7h",
    unread: 1,
  },
  {
    id: "group-1",
    title: "Team Product",
    subtitle: "Đã thêm tài liệu design",
    lastMessage: "Mọi người có ý kiến gì không?",
    unread: 3,
  },
  {
    id: "friend-2",
    title: "Hùng",
    subtitle: "Đang online",
    lastMessage: "Mai nhé, tớ bận họp xong sẽ gọi lại",
    unread: 0,
  },
  {
    id: "friend-1",
    title: "Mai",
    subtitle: "Đã gửi 2 ảnh",
    lastMessage: "Ok, mình sẽ qua chơi lúc 7h",
    unread: 1,
  },
  {
    id: "group-1",
    title: "Team Product",
    subtitle: "Đã thêm tài liệu design",
    lastMessage: "Mọi người có ý kiến gì không?",
    unread: 3,
  },
  {
    id: "friend-2",
    title: "Hùng",
    subtitle: "Đang online",
    lastMessage: "Mai nhé, tớ bận họp xong sẽ gọi lại",
    unread: 0,
  },
  {
    id: "friend-1",
    title: "Mai",
    subtitle: "Đã gửi 2 ảnh",
    lastMessage: "Ok, mình sẽ qua chơi lúc 7h",
    unread: 1,
  },
  {
    id: "group-1",
    title: "Team Product",
    subtitle: "Đã thêm tài liệu design",
    lastMessage: "Mọi người có ý kiến gì không?",
    unread: 3,
  },
];

export const initialMessages: Record<string, Message[]> = {
  "friend-1": [
    { from: "them", text: "Mình ghé uống cà phê chiều nay nhé?" },
    { from: "me", text: "Ok, mình sẽ qua chơi lúc 7h" },
  ],
  "group-1": [
    { from: "them", text: "Mọi người có ý kiến gì không?" },
    { from: "me", text: "Mình thấy nên chốt bản roadmap trước." },
  ],
  "friend-2": [
    { from: "them", text: "Nhớ mang bản thuyết trình nhé." },
    { from: "me", text: "Ok, tớ chuẩn bị xong rồi." },
  ],
};
