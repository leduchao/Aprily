import { useNavigate } from "@tanstack/react-router";
import type { TFunction } from "i18next";
import { useTranslation } from "react-i18next";
import z from "zod";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation } from "@tanstack/react-query";
import { enqueueSnackbar } from "notistack";

import { useAuthStore } from "../../stores/authStore";
import { signIn } from "../../services/auth";
import { toast } from "../../utils/toast";
import { PasswordInput } from "../../components/common/password-input/index-v2";

const signInSchema = (t: TFunction) =>
  z.object({
    email: z.email(t("validation.invalidEmail")),
    password: z.string().min(8, t("validation.passwordMinLength")),
  });

type SignInFormData = z.infer<ReturnType<typeof signInSchema>>;

export default function SignInPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();

  const [rememberMe, setRememberMe] = useState(false);

  const { setAuth } = useAuthStore.getState();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<SignInFormData>({
    resolver: zodResolver(signInSchema(t)),
    defaultValues: {
      email: "",
      password: "",
    },
  });

  const mutation = useMutation({
    mutationFn: signIn,
    onSuccess: (response) => {
      const accessToken = response.data?.accessToken;
      const user = response.data?.user;

      if (accessToken && user) {
        setAuth(user, accessToken);

        toast.pushToast({
          message: "You signed in",
          variant: "success",
        });

        navigate({
          to: "/",
          replace: true,
        });
      }
    },
    onError: (error) => {
      enqueueSnackbar(error.message, {
        variant: "error",
      });
    },
  });

  const onSubmit = async (data: SignInFormData) => {
    await mutation.mutateAsync({ ...data, rememberMe });
  };

  useEffect(() => {
    toast.showToast();
  }, []);

  return (
    <div className="min-vh-100 d-flex justify-content-center align-items-center">
      <div className="w-100 p-3" style={{ maxWidth: 420 }}>
        <div className="mb-4">
          <h1 className="fw-bold mb-1">{t("signIn.title")}</h1>
          <small className="text-muted">{t("signIn.subtitle")}</small>
        </div>

        <form
          onSubmit={handleSubmit(onSubmit)}
          className="d-flex flex-column gap-3"
        >
          <div>
            <label htmlFor="email" className="form-label">
              {t("signIn.email")}
            </label>

            <input
              id="email"
              type="email"
              className={`form-control ${errors.email ? "is-invalid" : ""}`}
              {...register("email")}
            />

            {errors.email && (
              <div className="invalid-feedback">{errors.email.message}</div>
            )}
          </div>

          <div>
            <PasswordInput
              label={t("signIn.password")}
              error={!!errors.password}
              helperText={errors.password?.message}
              {...register("password")}
            />
          </div>

          <div className="form-check">
            <input
              id="rememberMe"
              name="rememberMe"
              type="checkbox"
              className="form-check-input"
              checked={rememberMe}
              onChange={(e) => setRememberMe(e.target.checked)}
            />

            <label htmlFor="rememberMe" className="form-check-label">
              {t("common.rememberMe")}
            </label>
          </div>

          <button
            type="submit"
            className="btn btn-primary w-100 fw-bold text-uppercase"
            disabled={isSubmitting || mutation.isPending}
          >
            {mutation.isPending ? t("signIn.loading") : t("signIn.submit")}
          </button>

          <a
            href="/forgot-password"
            className="text-decoration-none text-end small"
          >
            {t("common.forgotPassword")}
          </a>

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
          {t("signIn.noAccount")}{" "}
          <a href="/sign-up" className="text-decoration-none">
            {t("signIn.signUpLink")}
          </a>
        </p>
      </div>
    </div>
  );
}
