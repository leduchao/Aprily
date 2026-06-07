import "./index.css";
import { forwardRef, useId, useState, type InputHTMLAttributes } from "react";
import { useTranslation } from "react-i18next";

interface Props extends InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  helperText?: string;
  error?: boolean;
}

export const PasswordInput = forwardRef<HTMLInputElement, Props>(
  function PasswordInput(
    { label, helperText, error, id, className = "", ...props },
    ref,
  ) {
    const { t } = useTranslation();

    const generatedId = useId();
    const inputId = id || `${generatedId}-input`;

    const finalLabel = label ?? t("common.password");

    const [showPassword, setShowPassword] = useState(false);

    return (
      <div>
        <label htmlFor={inputId} className="form-label">
          {finalLabel}
        </label>

        <div className="position-relative">
          <input
            {...props}
            ref={ref}
            id={inputId}
            type={showPassword ? "text" : "password"}
            className={`form-control pe-5 password-input ${error ? "is-invalid" : ""} ${className}`}
          />

          <button
            type="button"
            tabIndex={-1}
            className="btn position-absolute top-50 end-0 translate-middle-y border-0 bg-transparent px-3"
            onClick={() => setShowPassword((prev) => !prev)}
            onMouseDown={(e) => e.preventDefault()}
            aria-label={
              showPassword ? "hide the password" : "display the password"
            }
            style={{ zIndex: 5 }}
          >
            {showPassword ? (
              <i className="bi bi-eye-slash"></i>
            ) : (
              <i className="bi bi-eye"></i>
            )}
          </button>
        </div>

        {helperText && (
          <div className={error ? "invalid-feedback d-block" : "form-text"}>
            {helperText}
          </div>
        )}
      </div>
    );
  },
);
