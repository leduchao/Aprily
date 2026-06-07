import { createFileRoute, useNavigate } from "@tanstack/react-router";
import {
  Box,
  Button,
  Divider,
  Link,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import GoogleIcon from "@mui/icons-material/Google";
import FacebookIcon from "@mui/icons-material/Facebook";
import { useMutation } from "@tanstack/react-query";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslation } from "react-i18next";
import { useAuthStore } from "../../../stores/authStore";
import { signUp } from "../../../services/auth";
import type { TFunction } from "i18next";
import z from "zod";
import { PasswordInput } from "../../../components/common/password-input";
import SignUpPage from "../../../pages/sign-up";

export const Route = createFileRoute("/_guest/sign-up/")({
  component: SignUpPage,
});

const signUpSchema = (t: TFunction) =>
  z
    .object({
      fullName: z.string().min(2, t("validation.fullNameMinLength")),
      username: z
        .string()
        .min(3, t("validation.usernameMinLength"))
        .regex(/^[a-zA-Z0-9_]+$/, t("validation.usernameInvalid")),
      email: z.email(t("validation.invalidEmail")),
      password: z.string().min(8, t("validation.passwordMinLength")),
      confirmPassword: z
        .string()
        .min(8, t("validation.confirmPasswordRequired")),
    })
    .refine((data) => data.password === data.confirmPassword, {
      message: t("validation.passwordsDoNotMatch"),
      path: ["confirmPassword"],
    });

type SignUpFormData = z.infer<ReturnType<typeof signUpSchema>>;

function RouteComponent() {
  const { t } = useTranslation();
  const navigate = useNavigate();

  const setAuth = useAuthStore((state) => state.setAuth);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<SignUpFormData>({
    resolver: zodResolver(signUpSchema(t)),
    defaultValues: {
      fullName: "",
      username: "",
      email: "",
      password: "",
      confirmPassword: "",
    },
  });

  const mutation = useMutation({
    mutationFn: signUp,
    onSuccess: (response) => {
      const accessToken = response.data?.accessToken;
      const user = response.data?.user;

      if (accessToken && user) {
        setAuth(user, accessToken);
        navigate({ to: "/", replace: true });
      }
    },
    onError: (error) => {
      alert(error instanceof Error ? error.message : t("common.error"));
    },
  });

  const onSubmit = async (data: SignUpFormData) => {
    const { confirmPassword, ...payload } = data;
    await mutation.mutateAsync(payload);
  };

  return (
    <Box
      sx={{
        minHeight: "100vh",
        width: "100%",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
      }}
    >
      <Box
        sx={{
          width: "100%",
          p: 2,
        }}
      >
        <Box sx={{ marginBottom: "32px" }}>
          <Typography variant="h4" sx={{ fontWeight: "700" }}>
            {t("signUp.title")}
          </Typography>
          <Typography variant="caption">{t("signUp.subtitle")}</Typography>
        </Box>

        <Stack
          spacing={2}
          sx={{ marginTop: "16px" }}
          component={"form"}
          onSubmit={handleSubmit(onSubmit)}
        >
          <Stack
            direction={{ xs: "column", sm: "row" }}
            spacing={2}
            sx={{ width: "100%" }}
          >
            <TextField
              fullWidth
              id="fullName"
              label={t("signUp.fullName")}
              error={!!errors.fullName}
              helperText={
                errors.fullName?.message || t("signUp.fullNameExample")
              }
              {...register("fullName")}
            />

            <TextField
              fullWidth
              id="username"
              label={t("signUp.username")}
              error={!!errors.username}
              helperText={
                errors.username?.message || t("signUp.usernameExample")
              }
              {...register("username")}
            />
          </Stack>

          <TextField
            id="email"
            label={t("signUp.email")}
            type="email"
            error={!!errors.email}
            helperText={errors.email?.message || t("signUp.emailExample")}
            {...register("email")}
          />

          <Stack
            direction={{ xs: "column", sm: "row" }}
            spacing={2}
            sx={{ width: "100%" }}
          >
            <PasswordInput
              fullWidth
              id="password"
              label={t("signUp.password")}
              type="password"
              error={!!errors.password}
              helperText={
                errors.password?.message || "Must be at least 8 characters long"
              }
              {...register("password")}
            />

            <PasswordInput
              fullWidth
              id="confirmPassword"
              label={t("signUp.confirmPassword")}
              type="password"
              error={!!errors.confirmPassword}
              helperText={
                errors.confirmPassword?.message ||
                t("signUp.confirmPasswordHelper")
              }
              {...register("confirmPassword")}
            />
          </Stack>

          <Button
            type="submit"
            variant="contained"
            disabled={isSubmitting || mutation.isPending}
          >
            {t("signUp.submit")}
          </Button>

          <Divider>{t("common.or")}</Divider>

          <Button
            variant="outlined"
            startIcon={<GoogleIcon />}
            onClick={() => alert(t("common.signInWithGoogle"))}
          >
            {t("common.signInWithGoogle")}
          </Button>
          <Button
            variant="outlined"
            startIcon={<FacebookIcon />}
            onClick={() => alert(t("common.signInWithFacebook"))}
          >
            {t("common.signInWithFacebook")}
          </Button>
        </Stack>

        <Typography
          variant="body2"
          sx={{ textAlign: "center", marginTop: "24px" }}
        >
          {t("signUp.haveAccount")}{" "}
          <Link href="/sign-in" underline="none">
            {t("signUp.signInLink")}
          </Link>
        </Typography>
      </Box>
    </Box>
  );
}
