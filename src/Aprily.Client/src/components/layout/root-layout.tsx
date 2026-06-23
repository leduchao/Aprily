import { Outlet } from "@tanstack/react-router"

import { MobileOnly } from "@/components/layout/mobile-only"
import { ThemeProvider } from "@/components/theme-provider"
import { Toaster } from "@/components/ui/sonner"

export const RootLayout = () => {
  return (
    <ThemeProvider>
      <MobileOnly>
        <Outlet />
      </MobileOnly>
      <Toaster position="top-center" richColors />

      {/* {import.meta.env.DEV && (
        <>
          <TanStackRouterDevtools position="bottom-right" />
          <ReactQueryDevtools buttonPosition="bottom-left" />
        </>
      )} */}
    </ThemeProvider>
  )
}
