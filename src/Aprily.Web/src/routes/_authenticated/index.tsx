import { createFileRoute } from "@tanstack/react-router";
import { Box } from "@mui/material";

import { ThreadList } from "../../components/thread/thread-list";
import { NavigationFooter } from "../../components/home/navigation-footer";
import { chatThreads } from "../../data/chat";
import { HomeHeader } from "../../components/home/home-header";
import { useEffect } from "react";
import { toast } from "../../utils/toast";
import HomePage from "../../pages/home";

export const Route = createFileRoute("/_authenticated/")({
  component: HomePage,
});

function RouteComponent() {
  useEffect(() => {
    toast.showToast();
  }, []);

  const handleSearchThreads = (keyword: string) => {
    console.log(`Searching for ${keyword}...`);
  };

  return (
    <Box
      sx={{
        height: "100vh",
        overflow: "hidden",
        display: "flex",
        flexDirection: "column",
      }}
    >
      <HomeHeader onSearch={handleSearchThreads} />

      <ThreadList threads={chatThreads} />

      <NavigationFooter />
    </Box>
  );
}
