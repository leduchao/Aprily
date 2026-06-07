import type { UserProfile } from "./user-profile";

export interface SignInResponse {
  accessToken: string;
  user: UserProfile;
}

export interface SignUpResponse {
  accessToken: string;
  user: UserProfile;
}
