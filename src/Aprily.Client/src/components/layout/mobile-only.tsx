import type { ReactNode } from "react"

type MobileOnlyProps = {
  children: ReactNode
}

export const MobileOnly = ({ children }: MobileOnlyProps) => {
  return (
    <>
      <div className="block md:hidden">{children}</div>

      <main className="hidden min-h-screen items-center justify-center bg-background px-6 text-center md:flex">
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
