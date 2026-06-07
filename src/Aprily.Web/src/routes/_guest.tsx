import { createFileRoute, Outlet, redirect } from "@tanstack/react-router";
import { useAuthStore } from "../stores/authStore";

export const Route = createFileRoute("/_guest")({
  beforeLoad: () => {
    const { accessToken } = useAuthStore.getState();

    if (accessToken) {
      throw redirect({
        to: "/",
      });
    }
  },
  component: Outlet,
});
