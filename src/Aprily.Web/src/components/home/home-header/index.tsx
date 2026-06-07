import {
  Box,
  InputAdornment,
  OutlinedInput,
  Stack,
  Typography,
} from "@mui/material";
import { CreateThreadButton } from "./create-thread-button";
import { AvatarDrawer } from "./avatar-drawer";
import SearchIcon from "@mui/icons-material/Search";
import { useTranslation } from "react-i18next";
import { useEffect, useRef } from "react";
import type { ChangeEvent } from "react";

interface Props {
  onSearch: (keyword: string) => void;
}

export const HomeHeader = ({ onSearch }: Props) => {
  const { t } = useTranslation();

  const debounceTimeout = useRef<number | null>(null);

  const handleSearch = (
    e: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
  ) => {
    const value = e.target.value;
    if (!value) {
      return;
    }

    if (debounceTimeout.current) {
      window.clearTimeout(debounceTimeout.current);
    }
    debounceTimeout.current = window.setTimeout(() => {
      onSearch(value);
    }, 500);
  };

  useEffect(() => {
    return () => {
      if (debounceTimeout.current) {
        window.clearTimeout(debounceTimeout.current);
      }
    };
  }, []);

  return (
    <Box sx={{ p: 2 }}>
      <Stack
        direction={"row"}
        sx={{ justifyContent: "space-between", alignItems: "center", mb: 2 }}
      >
        <Typography variant="h4" sx={{ fontWeight: 700 }}>
          aprily
        </Typography>

        <Stack direction={"row"} spacing={1} sx={{ alignItems: "center" }}>
          <CreateThreadButton />
          <AvatarDrawer />
        </Stack>
      </Stack>

      <OutlinedInput
        size="small"
        fullWidth
        placeholder={t("home.searchPlaceholder")}
        startAdornment={
          <InputAdornment position="start">
            <SearchIcon />
          </InputAdornment>
        }
        onChange={handleSearch}
      />
    </Box>
  );
};
