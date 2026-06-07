import { Box, Stack } from "@mui/material";
import { Link } from "@tanstack/react-router";
import ChatBubbleIcon from "@mui/icons-material/ChatBubble";
import NotificationsIcon from "@mui/icons-material/Notifications";
import MenuIcon from "@mui/icons-material/Menu";

const navItems = [
  {
    to: "/",
    icon: <ChatBubbleIcon />,
  },
  {
    to: "/notifications",
    icon: <NotificationsIcon />,
  },
  {
    to: "/menu",
    icon: <MenuIcon />,
  },
];

export const NavigationFooter = () => {
  return (
    <Box
      sx={{
        p: 2,
        display: "flex",
        justifyContent: "center",
      }}
    >
      <Stack
        direction="row"
        sx={{
          justifyContent: "space-evenly",
          alignItems: "center",

          bgcolor: "primary.main",
          borderRadius: 999,

          width: "50%",
          py: 2,
        }}
      >
        {navItems.map(({ to, icon }) => (
          <Link
            key={to}
            to={to}
            style={{
              color: "white",
              display: "flex",
            }}
          >
            {icon}
          </Link>
        ))}
      </Stack>
    </Box>
  );
};
