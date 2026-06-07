import { api } from "./api";
import type {
  SignInPayload,
  SignUpPayload,
} from "../models/users/auth-payloads";
import type {
  SignInResponse,
  SignUpResponse,
} from "../models/users/auth-response";

export const signIn = (payload: SignInPayload) =>
  api.post<SignInResponse>("/users/auth/sign-in", {
    body: payload,
  });

export const signUp = (payload: SignUpPayload) =>
  api.post<SignUpResponse>("/users/auth/sign-up", {
    body: payload,
  });

export const signOut = () => api.post("/users/auth/sign-out");
