import { enqueueSnackbar } from "notistack";

export type ToastPayload = {
  message: string;
  variant: "success" | "error" | "warning" | "info";
};

function pushToast(toast: ToastPayload) {
  sessionStorage.setItem("toast", JSON.stringify(toast));
}

function popToast(): ToastPayload | null {
  const raw = sessionStorage.getItem("toast");

  if (!raw) return null;

  sessionStorage.removeItem("toast");

  return JSON.parse(raw);
}

function showToast() {
  const toast = popToast();

  if (!toast) return;

  enqueueSnackbar(toast.message, {
    variant: toast.variant,
  });
}

export const toast = {
  pushToast,
  popToast,
  showToast,
};
