export interface UserProfile {
  id: string;
  username: string;
  fullName?: string;
  email: string;
  avatarUrl?: string;
  lastLoginAt?: string;
  isEmailVerified?: boolean;
}
