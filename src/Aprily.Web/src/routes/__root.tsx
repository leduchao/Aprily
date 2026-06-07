import { createRootRoute, Outlet } from "@tanstack/react-router";
// import { CssBaseline, ThemeProvider, Box } from "@mui/material";
// import { MobileOnly } from "../components/layout/mobile-only";
import { SnackbarProvider } from "notistack";
import { useEffect } from "react";
import { toast } from "../utils/toast";
import { MobileOnly } from "../components/layout/mobile-only/index2";
// import { theme } from "../theme/theme";

export const Route = createRootRoute({
  component: RootComponent,
});

function RootComponent() {
  useEffect(() => {
    toast.showToast();
  }, []);

  return (
    // <ThemeProvider theme={theme}>
    //   <CssBaseline />

    //   <SnackbarProvider
    //     dense
    //     anchorOrigin={{
    //       horizontal: "right",
    //       vertical: "top",
    //     }}
    //   >
    //     <Box sx={{ position: "relative", minWidth: "375px" }}>
    //       <MobileOnly>
    //         <Outlet />
    //       </MobileOnly>
    //     </Box>
    //   </SnackbarProvider>
    // </ThemeProvider>

    <SnackbarProvider
      dense
      anchorOrigin={{
        horizontal: "right",
        vertical: "top",
      }}
    >
      <div style={{ position: "relative", minWidth: "375px" }}>
        <MobileOnly>
          <Outlet />
        </MobileOnly>
      </div>
    </SnackbarProvider>
  );
}
