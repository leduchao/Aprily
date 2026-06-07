import { useEffect, useState, type ReactNode } from "react";
import { useTranslation } from "react-i18next";

interface MobileOnlyProps {
  children: ReactNode;
}

export const MobileOnly = ({ children }: MobileOnlyProps) => {
  const [isMobile, setIsMobile] = useState(true);
  const [isLoaded, setIsLoaded] = useState(false);
  const { t } = useTranslation();

  useEffect(() => {
    const checkMobile = () => {
      setIsMobile(window.innerWidth <= 768);
    };

    checkMobile();
    setIsLoaded(true);

    window.addEventListener("resize", checkMobile);
    return () => window.removeEventListener("resize", checkMobile);
  }, []);

  if (!isLoaded) {
    return null;
  }

  if (!isMobile) {
    return (
      <div className="min-vh-100 d-flex align-items-center justify-content-center bg-body">
        <div className="container">
          <div className="row justify-content-center">
            <div className="col-12 col-sm-8 col-md-6">
              <div className="text-center py-4">
                <h1 className="h4 mb-3">{t("common.mobileOnly")}</h1>

                <p className="text-secondary mb-0">
                  {t("common.mobileOnlyDescription")}
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  return <>{children}</>;
};
