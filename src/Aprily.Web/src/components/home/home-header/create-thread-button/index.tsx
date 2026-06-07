import {
  Box,
  Chip,
  ListItemIcon,
  ListItemText,
  Menu,
  MenuItem,
} from "@mui/material";
import { useState } from "react";
import AddIcon from "@mui/icons-material/Add";
import AddCommentIcon from "@mui/icons-material/AddComment";
import GroupAddIcon from "@mui/icons-material/GroupAdd";

export const CreateThreadButton = () => {
  const [createMenuAnchorEl, setCreateMenuAnchorEl] =
    useState<null | HTMLElement>(null);

  const open = Boolean(createMenuAnchorEl);

  const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setCreateMenuAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setCreateMenuAnchorEl(null);
  };
  return (
    <Box>
      <Chip
        component={"button"}
        icon={<AddIcon />}
        label="create"
        color="primary"
        sx={{
          textTransform: "uppercase",
          fontWeight: 700,
        }}
        onClick={handleClick}
      />
      <Menu
        anchorEl={createMenuAnchorEl}
        open={open}
        onClose={handleClose}
        anchorOrigin={{
          vertical: "bottom",
          horizontal: "left",
        }}
        transformOrigin={{
          vertical: "top",
          horizontal: "center",
        }}
      >
        <MenuItem onClick={handleClose} divider dense>
          <ListItemIcon sx={{ color: "common.black" }}>
            <AddCommentIcon />
          </ListItemIcon>
          <ListItemText>Chat</ListItemText>
        </MenuItem>
        <MenuItem onClick={handleClose} dense>
          <ListItemIcon sx={{ color: "common.black" }}>
            <GroupAddIcon />
          </ListItemIcon>
          <ListItemText>Group</ListItemText>
        </MenuItem>
      </Menu>
    </Box>
  );
};
