import { Outlet } from "@tanstack/react-router"
import { ReactQueryDevtools } from "@tanstack/react-query-devtools"
import { TanStackRouterDevtools } from "@tanstack/react-router-devtools"

import { MobileOnly } from "@/components/layout/mobile-only"
import { ThemeProvider } from "@/components/theme-provider"

export const RootLayout = () => {
  return (
    <ThemeProvider defaultTheme="light">
      <MobileOnly>
        <Outlet />
      </MobileOnly>

      {import.meta.env.DEV && (
        <>
          <TanStackRouterDevtools position="bottom-right" />
          <ReactQueryDevtools buttonPosition="bottom-left" />
        </>
      )}
    </ThemeProvider>
  )
}
