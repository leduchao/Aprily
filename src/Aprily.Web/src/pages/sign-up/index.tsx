import { useNavigate } from "@tanstack/react-router";
import { useMutation } from "@tanstack/react-query";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslation } from "react-i18next";
import type { TFunction } from "i18next";
import z from "zod";

import { useAuthStore } from "../../stores/authStore";
import { signUp } from "../../services/auth";
import { PasswordInput } from "../../components/common/password-input/index-v2";

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

export default function SignUpPage() {
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
    <div className="min-vh-100 w-100 d-flex justify-content-center align-items-center">
      <div className="w-100 p-3" style={{ maxWidth: 520 }}>
        <div className="mb-4">
          <h1 className="fw-bold mb-1">{t("signUp.title")}</h1>
          <small className="text-muted">{t("signUp.subtitle")}</small>
        </div>

        <form
          onSubmit={handleSubmit(onSubmit)}
          className="d-flex flex-column gap-3"
        >
          <div className="row g-3">
            <div className="col-12 col-sm-6">
              <label htmlFor="fullName" className="form-label">
                {t("signUp.fullName")}
              </label>

              <input
                id="fullName"
                type="text"
                className={`form-control ${
                  errors.fullName ? "is-invalid" : ""
                }`}
                {...register("fullName")}
              />

              <div
                className={
                  errors.fullName ? "invalid-feedback d-block" : "form-text"
                }
              >
                {errors.fullName?.message || t("signUp.fullNameExample")}
              </div>
            </div>

            <div className="col-12 col-sm-6">
              <label htmlFor="username" className="form-label">
                {t("signUp.username")}
              </label>

              <input
                id="username"
                type="text"
                className={`form-control ${
                  errors.username ? "is-invalid" : ""
                }`}
                {...register("username")}
              />

              <div
                className={
                  errors.username ? "invalid-feedback d-block" : "form-text"
                }
              >
                {errors.username?.message || t("signUp.usernameExample")}
              </div>
            </div>
          </div>

          <div>
            <label htmlFor="email" className="form-label">
              {t("signUp.email")}
            </label>

            <input
              id="email"
              type="email"
              className={`form-control ${errors.email ? "is-invalid" : ""}`}
              {...register("email")}
            />

            <div
              className={
                errors.email ? "invalid-feedback d-block" : "form-text"
              }
            >
              {errors.email?.message || t("signUp.emailExample")}
            </div>
          </div>

          <div className="row g-3">
            <div className="col-12 col-sm-6">
              <PasswordInput
                id="password"
                label={t("signUp.password")}
                error={!!errors.password}
                helperText={
                  errors.password?.message ||
                  "Must be at least 8 characters long"
                }
                {...register("password")}
              />
            </div>

            <div className="col-12 col-sm-6">
              <PasswordInput
                id="confirmPassword"
                label={t("signUp.confirmPassword")}
                error={!!errors.confirmPassword}
                helperText={
                  errors.confirmPassword?.message ||
                  t("signUp.confirmPasswordHelper")
                }
                {...register("confirmPassword")}
              />
            </div>
          </div>

          <button
            type="submit"
            className="btn btn-primary w-100"
            disabled={isSubmitting || mutation.isPending}
          >
            {mutation.isPending ? "Loading..." : t("signUp.submit")}
          </button>

          <div className="d-flex align-items-center gap-3 my-2">
            <hr className="flex-grow-1" />
            <span className="text-muted small">{t("common.or")}</span>
            <hr className="flex-grow-1" />
          </div>

          <button
            type="button"
            className="btn btn-outline-primary w-100"
            onClick={() => alert(t("common.signInWithGoogle"))}
          >
            <i className="me-2 bi bi-google"></i>
            {t("common.signInWithGoogle")}
          </button>

          <button
            type="button"
            className="btn btn-outline-primary w-100"
            onClick={() => alert(t("common.signInWithFacebook"))}
          >
            <i className="me-2 bi bi-facebook"></i>
            {t("common.signInWithFacebook")}
          </button>
        </form>

        <p className="text-center mt-4 mb-0 small">
          {t("signUp.haveAccount")}{" "}
          <a href="/sign-in" className="text-decoration-none">
            {t("signUp.signInLink")}
          </a>
        </p>
      </div>
    </div>
  );
}
