import { QueryClient } from "@tanstack/react-query"

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 100 * 60,
      retry: 1,
      // refetchOnWindowFocus: false,
    },
  },
})
