import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import en from "../locales/en.json";
import vi from "../locales/vi.json";

const resources = {
  en: { translation: en },
  vi: { translation: vi },
};

// Get default language from localStorage or browser language
const getDefaultLanguage = () => {
  const savedLanguage = localStorage.getItem("language");
  if (savedLanguage) return savedLanguage;

  const browserLanguage = navigator.language.split("-")[0];
  return ["en", "vi"].includes(browserLanguage) ? browserLanguage : "en";
};

i18n.use(initReactI18next).init({
  resources,
  lng: getDefaultLanguage(),
  fallbackLng: "en",
  ns: ["translation"],
  defaultNS: "translation",
  interpolation: {
    escapeValue: false,
  },
});

export default i18n;
