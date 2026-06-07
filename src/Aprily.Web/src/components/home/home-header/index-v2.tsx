import { useTranslation } from "react-i18next";
import { useEffect, useRef } from "react";
import type { ChangeEvent } from "react";
import { CreateThreadButton } from "./create-thread-button/index-v2";
import { AvatarDrawer } from "./avatar-drawer/index-v2";

interface Props {
  onSearch: (keyword: string) => void;
}

export const HomeHeader = ({ onSearch }: Props) => {
  const { t } = useTranslation();

  const debounceTimeout = useRef<number | null>(null);

  const handleSearch = (e: ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value.trim();

    if (debounceTimeout.current) {
      window.clearTimeout(debounceTimeout.current);
    }

    debounceTimeout.current = window.setTimeout(() => {
      onSearch(value);
    }, 500);
  };

  useEffect(() => {
    return () => {
      if (debounceTimeout.current) {
        window.clearTimeout(debounceTimeout.current);
      }
    };
  }, []);

  return (
    <header className="p-3">
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h1 className="fw-bold mb-0">aprily</h1>

        <div className="d-flex align-items-center gap-2">
          <CreateThreadButton />
          <AvatarDrawer />
        </div>
      </div>

      <div className="position-relative">
        <i className="bi bi-search position-absolute top-50 start-0 translate-middle-y ms-3 text-muted"></i>

        <input
          type="search"
          className="form-control ps-5"
          placeholder={t("home.searchPlaceholder")}
          onChange={handleSearch}
        />
      </div>
    </header>
  );
};
