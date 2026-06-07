import {
  Avatar,
  Box,
  Divider,
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Stack,
  Typography,
} from "@mui/material";
import { useState, type ReactNode } from "react";
import AccountCircleIcon from "@mui/icons-material/AccountCircle";
import SettingsIcon from "@mui/icons-material/Settings";
import LogoutIcon from "@mui/icons-material/Logout";
import HelpIcon from "@mui/icons-material/Help";
import { useAuthStore } from "../../../../stores/authStore";
import { useNavigate } from "@tanstack/react-router";
import { toast } from "../../../../utils/toast";
import { useMutation } from "@tanstack/react-query";
import { signOut } from "../../../../services/auth";
import { enqueueSnackbar } from "notistack";

type MenuIcon = {
  label: string;
  icon?: ReactNode;
  color?: string;
  onClick?: () => void;
};

export const AvatarDrawer = () => {
  const navigate = useNavigate();
  const { user, clearAuth } = useAuthStore.getState();

  const [isOpenAvatarDrawer, setIsOpenAvatarDrawer] = useState(false);
  const toggleDrawer = (open: boolean) => {
    setIsOpenAvatarDrawer(open);
  };

  const mutation = useMutation({
    mutationFn: signOut,
    onSuccess: () => {
      toast.pushToast({
        message: "You signed out",
        variant: "success",
      });
    },
    onError: (error) => {
      enqueueSnackbar({
        message: error.message,
        variant: "error",
      });
    },
  });

  const handleSignOut = async () => {
    await mutation.mutateAsync();
    clearAuth();
    navigate({ to: "/sign-in", replace: true });
  };

  const menuItems: MenuIcon[] = [
    {
      label: "Profile",
      icon: <AccountCircleIcon />,
      onClick: () => navigate({ to: `/profile/${user?.username}` }),
    },
    {
      label: "Settings",
      icon: <SettingsIcon />,
      onClick: () => navigate({ to: `/settings/${user?.username}` }),
    },
  ];

  const bottomItems: MenuIcon[] = [
    {
      label: "Sign out",
      icon: <LogoutIcon color="error" />,
      color: "error.main",
      onClick: handleSignOut,
    },
    {
      label: "Help",
      icon: <HelpIcon />,
      onClick: () => navigate({ to: "/help" }),
    },
  ];

  const renderItems = (items: typeof menuItems) => (
    <List>
      {items.map((item) => (
        <ListItem key={item.label} disablePadding>
          <ListItemButton onClick={item.onClick}>
            {item.icon && (
              <ListItemIcon sx={{ color: item.color || "common.black" }}>
                {item.icon}
              </ListItemIcon>
            )}
            <ListItemText primary={item.label} sx={{ color: item.color }} />
          </ListItemButton>
        </ListItem>
      ))}
    </List>
  );

  return (
    <Box>
      <Avatar
        alt={user?.fullName || user?.username}
        src={user?.avatarUrl}
        onClick={() => toggleDrawer(true)}
        sx={{ cursor: "pointer", color: "background.paper" }}
      >
        {user?.username.charAt(0).toUpperCase()}
      </Avatar>
      <Drawer
        open={isOpenAvatarDrawer}
        onClose={() => toggleDrawer(false)}
        anchor="right"
      >
        <Box
          sx={{ width: 250 }}
          role="presentation"
          onClick={() => toggleDrawer(false)}
        >
          <Stack
            direction={"row"}
            spacing={2}
            sx={{ p: 2, alignItems: "center", bgcolor: "background.default" }}
          >
            <Avatar
              alt={user?.fullName || user?.username}
              src={user?.avatarUrl}
              sx={{ width: 56, height: 56 }}
            >
              {user?.username.charAt(0).toUpperCase()}
            </Avatar>
            <Box>
              <Typography sx={{ fontWeight: 700 }}>
                {user?.fullName || user?.username}
              </Typography>
              <Typography variant="body2">@{user?.username}</Typography>
            </Box>
          </Stack>

          {renderItems(menuItems)}

          <Divider />

          {renderItems(bottomItems)}
        </Box>
      </Drawer>
    </Box>
  );
};
