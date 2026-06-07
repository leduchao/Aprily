import type { ButtonHTMLAttributes } from "react";

interface AvatarProps extends Omit<
  ButtonHTMLAttributes<HTMLButtonElement>,
  "name"
> {
  src?: string | null;
  alt?: string;
  name?: string | null;
  size?: number;
  clickable?: boolean;
}

export const Avatar = ({
  src,
  alt,
  name,
  size = 40,
  clickable = false,
  className = "",
  ...props
}: AvatarProps) => {
  const fallback = name?.charAt(0).toUpperCase() || "?";

  const fontSize = Math.round(size * 0.4);

  const content = src ? (
    <img
      src={src}
      alt={alt || name || "Avatar"}
      className="w-100 h-100 object-fit-cover"
    />
  ) : (
    <span
      className="d-flex w-100 h-100 align-items-center justify-content-center rounded-circle bg-primary text-white fw-bold"
      style={{ fontSize }}
    >
      {fallback}
    </span>
  );

  const commonStyle = {
    width: size,
    height: size,
    minWidth: size,
    minHeight: size,
  };

  if (clickable) {
    return (
      <button
        type="button"
        className={`btn p-0 border-0 rounded-circle overflow-hidden ${className}`}
        style={commonStyle}
        {...props}
      >
        {content}
      </button>
    );
  }

  return (
    <div
      className={`rounded-circle overflow-hidden ${className}`}
      style={commonStyle}
    >
      {content}
    </div>
  );
};
