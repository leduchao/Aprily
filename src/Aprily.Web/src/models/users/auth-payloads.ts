export type SignInPayload = {
  email: string;
  password: string;
  rememberMe?: boolean;
};

export type SignUpPayload = {
  fullName?: string;
  username: string;
  email: string;
  password: string;
};
