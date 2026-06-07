import { useTranslation } from "react-i18next";
import { Button, Stack } from "@mui/material";

export const LanguageSwitcher = () => {
  const { i18n } = useTranslation();

  const handleLanguageChange = (lng: string) => {
    i18n.changeLanguage(lng);
    localStorage.setItem("language", lng);
  };

  return (
    <Stack
      direction="row"
      spacing={1}
      sx={{
        position: "absolute",
        top: 16,
        right: 16,
        zIndex: 1000,
      }}
    >
      <Button
        variant={i18n.language === "en" ? "contained" : "outlined"}
        size="small"
        onClick={() => handleLanguageChange("en")}
      >
        EN
      </Button>
      <Button
        variant={i18n.language === "vi" ? "contained" : "outlined"}
        size="small"
        onClick={() => handleLanguageChange("vi")}
      >
        VI
      </Button>
    </Stack>
  );
};
