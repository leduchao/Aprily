import { useEffect, useState, type ReactNode } from "react";
import { Box, Typography, Container } from "@mui/material";
import { useTranslation } from "react-i18next";

interface MobileOnlyProps {
  children: ReactNode;
}

export const MobileOnly = ({ children }: MobileOnlyProps) => {
  const [isMobile, setIsMobile] = useState(true);
  const [isLoaded, setIsLoaded] = useState(false);
  const { t } = useTranslation();

  useEffect(() => {
    // Check on mount
    const checkMobile = () => {
      setIsMobile(window.innerWidth <= 768);
    };

    checkMobile();
    setIsLoaded(true);

    // Listen for resize events
    window.addEventListener("resize", checkMobile);
    return () => window.removeEventListener("resize", checkMobile);
  }, []);

  if (!isLoaded) {
    return null;
  }

  if (!isMobile) {
    return (
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          minHeight: "100vh",
          backgroundColor: "background.default",
        }}
      >
        <Container maxWidth="sm">
          <Box sx={{ textAlign: "center", py: 4 }}>
            <Typography variant="h4" component="h1" gutterBottom>
              {t("common.mobileOnly")}
            </Typography>
            <Typography variant="body1" color="text.secondary" sx={{ mt: 2 }}>
              {t("common.mobileOnlyDescription")}
            </Typography>
          </Box>
        </Container>
      </Box>
    );
  }

  return <>{children}</>;
};
