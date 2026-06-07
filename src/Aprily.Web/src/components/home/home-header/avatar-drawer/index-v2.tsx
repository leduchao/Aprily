import { useState, type ReactNode } from "react";
import { useNavigate } from "@tanstack/react-router";
import { useMutation } from "@tanstack/react-query";
import { enqueueSnackbar } from "notistack";

import { useAuthStore } from "../../../../stores/authStore";
import { toast } from "../../../../utils/toast";
import { signOut } from "../../../../services/auth";
import { Avatar } from "../../../common/avatar";

type MenuItem = {
  label: string;
  icon?: ReactNode;
  colorClass?: string;
  onClick?: () => void;
};

export const AvatarDrawer = () => {
  const navigate = useNavigate();
  const { user, clearAuth } = useAuthStore.getState();

  const [isOpenAvatarDrawer, setIsOpenAvatarDrawer] = useState(false);

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

  const handleClose = () => {
    setIsOpenAvatarDrawer(false);
  };

  const handleSignOut = async () => {
    await mutation.mutateAsync();
    clearAuth();
    navigate({ to: "/sign-in", replace: true });
  };

  const menuItems: MenuItem[] = [
    {
      label: "Profile",
      icon: <i className="bi bi-person-circle"></i>,
      onClick: () => navigate({ to: `/profile/${user?.username}` }),
    },
    {
      label: "Settings",
      icon: <i className="bi bi-gear"></i>,
      onClick: () => navigate({ to: `/settings/${user?.username}` }),
    },
  ];

  const bottomItems: MenuItem[] = [
    {
      label: "Sign out",
      icon: <i className="bi bi-box-arrow-right"></i>,
      colorClass: "text-danger",
      onClick: handleSignOut,
    },
    {
      label: "Help",
      icon: <i className="bi bi-question-circle"></i>,
      onClick: () => navigate({ to: "/help" }),
    },
  ];

  const handleItemClick = async (onClick?: () => void | Promise<void>) => {
    handleClose();
    await onClick?.();
  };

  const renderItems = (items: MenuItem[]) => (
    <div className="list-group list-group-flush">
      {items.map((item) => (
        <button
          key={item.label}
          type="button"
          className={`list-group-item list-group-item-action d-flex align-items-center gap-3 border-0 ${
            item.colorClass || ""
          }`}
          onClick={() => handleItemClick(item.onClick)}
          disabled={mutation.isPending && item.label === "Sign out"}
        >
          <span
            className={`fs-5 d-inline-flex ${item.colorClass || "text-body"}`}
            style={{ width: 24 }}
          >
            {item.icon}
          </span>

          <span className="fw-medium">
            {mutation.isPending && item.label === "Sign out"
              ? "Signing out..."
              : item.label}
          </span>
        </button>
      ))}
    </div>
  );

  return (
    <>
      <Avatar
        clickable
        src={user?.avatarUrl}
        alt={user?.fullName || user?.username}
        name={user?.fullName || user?.username}
        size={40}
        onClick={() => setIsOpenAvatarDrawer(true)}
        aria-label="Open user menu"
      />

      {isOpenAvatarDrawer && (
        <div
          className="offcanvas-backdrop fade show"
          onClick={handleClose}
        ></div>
      )}

      <div
        className={`offcanvas offcanvas-end ${
          isOpenAvatarDrawer ? "show" : ""
        }`}
        tabIndex={-1}
        style={{
          visibility: isOpenAvatarDrawer ? "visible" : "hidden",
          width: 250,
        }}
      >
        <div className="d-flex align-items-center gap-3 p-3 bg-body-tertiary">
          <Avatar
            src={user?.avatarUrl}
            alt={user?.fullName || user?.username}
            name={user?.fullName || user?.username}
            size={56}
          />

          <div className="overflow-hidden">
            <div className="fw-bold text-truncate">
              {user?.fullName || user?.username}
            </div>
            <div className="small text-muted text-truncate">
              @{user?.username}
            </div>
          </div>
        </div>

        {renderItems(menuItems)}

        <hr className="my-0" />

        {renderItems(bottomItems)}
      </div>
    </>
  );
};
