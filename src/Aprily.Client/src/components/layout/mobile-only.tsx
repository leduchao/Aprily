import type { ReactNode } from "react"

type MobileOnlyProps = {
  children: ReactNode
}

export const MobileOnly = ({ children }: MobileOnlyProps) => {
  return (
    <>
      <div className="block h-dvh w-full overflow-x-auto overflow-y-hidden bg-background md:hidden">
        <div className="h-full w-full min-w-[360px] overflow-hidden text-foreground">
          {children}
        </div>
      </div>

      <main className="hidden min-h-dvh items-center justify-center bg-background px-6 text-center text-foreground md:flex">
        <div className="max-w-sm space-y-3">
          <p className="text-lg font-semibold">Mobile screen only</p>

          <p className="text-sm text-muted-foreground">
            This app currently supports screens under 768px. Please open it on a
            mobile device or resize your browser.
          </p>
        </div>
      </main>
    </>
  )
}
