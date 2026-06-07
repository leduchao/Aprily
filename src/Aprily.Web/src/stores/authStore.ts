import { create } from "zustand";
import { persist } from "zustand/middleware";
import type { UserProfile } from "../models/users/user-profile";

interface AuthState {
  user: UserProfile | null;
  accessToken: string | null;
  setAuth: (user: UserProfile, accessToken: string) => void;
  clearAuth: () => void;
}

export const useAuthStore = create<AuthState>()(
  // store token in local storage
  persist(
    (set) => ({
      user: null,
      accessToken: null,
      setAuth: (user, accessToken) => set({ user, accessToken }),
      clearAuth: () => set({ user: null, accessToken: null }),
    }),
    { name: "auth-storage" }, // key in localStorage
  ),
);
