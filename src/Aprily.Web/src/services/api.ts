import { redirect } from "@tanstack/react-router";
import type { Result } from "../models/result";
import { useAuthStore } from "../stores/authStore";
import { enqueueSnackbar } from "notistack";
import { toast } from "../utils/toast";

const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL?.replace(/\/+$/, "") ?? "";

const API_VERSION = "v1";

export type ApiRequestOptions = {
  headers?: Record<string, string>;
  query?: Record<string, string | number | boolean | undefined>;
  body?: unknown;
  signal?: AbortSignal;
  credentials?: RequestCredentials;
};

function buildUrl(path: string, query?: ApiRequestOptions["query"]) {
  const url = path.startsWith("http")
    ? new URL(path)
    : new URL(`${API_BASE_URL}/${API_VERSION}${path}`, window.location.origin);

  if (query) {
    Object.entries(query).forEach(([key, value]) => {
      if (value === undefined || value === null) return;
      url.searchParams.set(key, String(value));
    });
  }

  return url.toString();
}

async function refreshAccessToken(): Promise<string | null> {
  try {
    const { setAuth, user, clearAuth } = useAuthStore.getState();

    const response = await fetch(buildUrl("/users/auth/refresh-token"), {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
    });

    if (!response.ok) {
      clearAuth();
      return null;
    }

    const result = (await response.json()) as Result<{
      accessToken: string;
      refreshToken: string;
    }>;

    if (result.isFailure || !result.data) {
      clearAuth();
      return null;
    }

    setAuth(user!, result.data.accessToken);

    return result.data.accessToken;
  } catch {
    return null;
  }
}

async function request<T>(
  method: string,
  path: string,
  options: ApiRequestOptions = {},
  retry = true,
): Promise<Result<T>> {
  const {
    headers = {},
    query,
    body,
    signal,
    credentials = "include",
  } = options;
  const url = buildUrl(path, query);

  const { accessToken, clearAuth } = useAuthStore.getState();

  const requestOptions: RequestInit = {
    method,
    headers: {
      "Content-Type": "application/json",
      ...(accessToken ? { Authorization: `Bearer ${accessToken}` } : {}),
      ...headers,
    },
    signal,
    credentials,
  };

  if (body != null && method !== "GET" && method !== "HEAD") {
    requestOptions.body = JSON.stringify(body);
  }

  let response = await fetch(url, requestOptions);

  // ===== INTERCEPTOR =====

  if (response.status === 401 && retry) {
    const newAccessToken = await refreshAccessToken();

    if (newAccessToken) {
      requestOptions.headers = {
        ...requestOptions.headers,
        Authorization: `Bearer ${newAccessToken}`,
      };

      response = await fetch(url, requestOptions);
    } else {
      clearAuth();

      toast.pushToast({
        message: "Your session expired, please sign in again",
        variant: "warning",
      });

      throw redirect({
        to: "/sign-in",
      });
    }
  }

  if (response.status === 403) {
    enqueueSnackbar("You do not have permission for this action", {
      variant: "warning",
    });
  }

  // =======================

  const result = (await response.json()) as Result<T>;

  if (!response.ok) {
    throw new Error(
      result.error?.message ??
        `API ${method} ${url} failed with ${response.status}`,
    );
  }

  if (result.isFailure) {
    throw new Error(result.error?.message);
  }

  return result;
}

export const api = {
  get: <T>(path: string, options?: Omit<ApiRequestOptions, "body">) =>
    request<T>("GET", path, options),
  post: <T>(path: string, options?: ApiRequestOptions) =>
    request<T>("POST", path, options),
  put: <T>(path: string, options?: ApiRequestOptions) =>
    request<T>("PUT", path, options),
  patch: <T>(path: string, options?: ApiRequestOptions) =>
    request<T>("PATCH", path, options),
  delete: <T>(path: string, options?: ApiRequestOptions) =>
    request<T>("DELETE", path, options),
};

export default api;
