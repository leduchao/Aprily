import { createFileRoute, Outlet, redirect } from "@tanstack/react-router";
import { useAuthStore } from "../stores/authStore";

export const Route = createFileRoute("/_authenticated")({
  beforeLoad: () => {
    const { accessToken } = useAuthStore.getState();

    if (!accessToken) {
      throw redirect({
        to: "/sign-in",
      });
    }
  },

  component: Outlet,
});
