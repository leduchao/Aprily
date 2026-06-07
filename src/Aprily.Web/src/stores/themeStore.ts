import type { PaletteMode } from "@mui/material";
import { create } from "zustand";
import { persist } from "zustand/middleware";

interface Theme {
  mode: PaletteMode;
  setTheme: (mode: PaletteMode) => void;
}

export const useThemeStore = create<Theme>()(
  persist(
    (set) => ({
      mode: "light",
      setTheme: (mode) => set({ mode }),
    }),
    { name: "theme" },
  ),
);
