import { StrictMode } from "react"
import { createRoot } from "react-dom/client"

import "./index.css"
import { QueryClientProvider } from "@tanstack/react-query"
import { queryClient } from "./query-client.ts"
import { RouterProvider } from "@tanstack/react-router"
import { router } from "./router.tsx"

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <RouterProvider router={router}></RouterProvider>
    </QueryClientProvider>
  </StrictMode>
)
