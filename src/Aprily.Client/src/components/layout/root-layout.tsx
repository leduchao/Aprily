import { Outlet } from "@tanstack/react-router"

import { MobileOnly } from "@/components/layout/mobile-only"
import { ThemeProvider } from "@/components/theme-provider"

export const RootLayout = () => {
  return (
    <ThemeProvider defaultTheme="dark">
      <MobileOnly>
        <Outlet />
      </MobileOnly>

      {/* {import.meta.env.DEV && (
        <>
          <TanStackRouterDevtools position="bottom-right" />
          <ReactQueryDevtools buttonPosition="bottom-left" />
        </>
      )} */}
    </ThemeProvider>
  )
}
