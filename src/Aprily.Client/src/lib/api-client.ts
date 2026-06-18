import { useAuthStore } from "@/lib/auth-store"

type ApiErrorBody = {
  code: string
  message: string
}

type ApiResult<TData> = {
  isSuccess: boolean
  isFailure: boolean
  error?: ApiErrorBody | null
  data?: TData | null
}

type ApiRequestOptions<TBody> = {
  body?: TBody
  headers?: HeadersInit
  signal?: AbortSignal
  auth?: boolean
  credentials?: RequestCredentials
}

type HttpMethod = "GET" | "POST" | "PUT" | "PATCH" | "DELETE"

const API_VERSION = "v1"
const configuredBaseUrl = import.meta.env.VITE_API_BASE_URL ?? "/api"
let refreshSessionPromise: Promise<string | null> | null = null

export class ApiError extends Error {
  code?: string
  status: number

  constructor(message: string, status: number, code?: string) {
    super(message)
    this.name = "ApiError"
    this.status = status
    this.code = code
  }
}

export const apiClient = {
  get: <TResponse>(path: string, options?: ApiRequestOptions<never>) =>
    request<TResponse, never>("GET", path, options),

  post: <TResponse, TBody = unknown>(
    path: string,
    body?: TBody,
    options?: Omit<ApiRequestOptions<TBody>, "body">
  ) => request<TResponse, TBody>("POST", path, { ...options, body }),

  put: <TResponse, TBody = unknown>(
    path: string,
    body?: TBody,
    options?: Omit<ApiRequestOptions<TBody>, "body">
  ) => request<TResponse, TBody>("PUT", path, { ...options, body }),

  patch: <TResponse, TBody = unknown>(
    path: string,
    body?: TBody,
    options?: Omit<ApiRequestOptions<TBody>, "body">
  ) => request<TResponse, TBody>("PATCH", path, { ...options, body }),

  delete: <TResponse>(path: string, options?: ApiRequestOptions<never>) =>
    request<TResponse, never>("DELETE", path, options),
}

const request = async <TResponse, TBody>(
  method: HttpMethod,
  path: string,
  options?: ApiRequestOptions<TBody>
) => {
  const response = await sendRequest(method, path, options)

  if (
    response.status === 401 &&
    options?.auth !== false &&
    !isRefreshRequest(path)
  ) {
    const accessToken = await refreshSession()

    if (accessToken) {
      return parseRequestResponse<TResponse>(
        await sendRequest(method, path, options, accessToken)
      )
    }
  }

  return parseRequestResponse<TResponse>(response)
}

const sendRequest = async <TBody>(
  method: HttpMethod,
  path: string,
  options?: ApiRequestOptions<TBody>,
  accessTokenOverride?: string
) => {
  const headers = new Headers(options?.headers)

  if (options?.body !== undefined && !headers.has("Content-Type")) {
    headers.set("Content-Type", "application/json")
  }

  const accessToken = accessTokenOverride ?? useAuthStore.getState().accessToken
  if (options?.auth !== false && accessToken && !headers.has("Authorization")) {
    headers.set("Authorization", `Bearer ${accessToken}`)
  }

  return fetch(resolveUrl(path), {
    method,
    headers,
    credentials: options?.credentials ?? "include",
    signal: options?.signal,
    body:
      options?.body === undefined ? undefined : JSON.stringify(options.body),
  })
}

const parseRequestResponse = async <TResponse>(response: Response) => {
  const payload = await parseResponse(response)

  if (isApiResult<TResponse>(payload)) {
    if (!response.ok || payload.isFailure) {
      throw new ApiError(
        payload.error?.message ?? "Unable to complete the request",
        response.status,
        payload.error?.code
      )
    }

    return payload.data as TResponse
  }

  if (!response.ok) {
    throw new ApiError("Unable to complete the request", response.status)
  }

  return payload as TResponse
}

const refreshSession = async () => {
  refreshSessionPromise ??= refreshSessionInternal().finally(() => {
    refreshSessionPromise = null
  })

  return refreshSessionPromise
}

const refreshSessionInternal = async () => {
  try {
    const response = await fetch(resolveUrl("/users/auth/refresh-token"), {
      method: "POST",
      credentials: "include",
    })

    const payload = await parseResponse(response)

    if (!response.ok || !isApiResult<AuthPayload>(payload) || payload.isFailure || !payload.data) {
      useAuthStore.getState().clearSession()
      return null
    }

    useAuthStore.getState().setSession(payload.data.accessToken, payload.data.user)
    return payload.data.accessToken
  } catch {
    useAuthStore.getState().clearSession()
    return null
  }
}

const parseResponse = async (response: Response) => {
  if (response.status === 204) {
    return null
  }

  const text = await response.text()
  if (!text) {
    return null
  }

  return JSON.parse(text) as unknown
}

const isApiResult = <TResponse>(
  payload: unknown
): payload is ApiResult<TResponse> => {
  return (
    typeof payload === "object" &&
    payload !== null &&
    "isSuccess" in payload &&
    "isFailure" in payload
  )
}

const resolveUrl = (path: string) => {
  if (/^https?:\/\//.test(path)) {
    return path
  }

  const baseUrl = normalizeBaseUrl(configuredBaseUrl)
  const normalizedPath = path.startsWith("/") ? path : `/${path}`

  return `${baseUrl}${normalizedPath}`
}

const normalizeBaseUrl = (baseUrl: string) => {
  const trimmedBaseUrl = baseUrl.replace(/\/+$/, "")

  return trimmedBaseUrl.endsWith(`/${API_VERSION}`)
    ? trimmedBaseUrl
    : `${trimmedBaseUrl}/${API_VERSION}`
}

const isRefreshRequest = (path: string) => {
  return path.includes("/users/auth/refresh-token")
}

type AuthPayload = {
  accessToken: string
  user: {
    id: string
    username: string
    fullName: string | null
    email: string
    avatarUrl: string | null
    lastSignInAt: string
    isEmailVerified: boolean
  }
}
