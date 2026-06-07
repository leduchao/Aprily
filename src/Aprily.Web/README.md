# 📱 Aprily Frontend

A mobile-first chat web app built with React, TypeScript, and Material-UI. Designed as a website with a mobile-only user interface, optimized for touch and small screens.

![React](https://img.shields.io/badge/React-19.2.5-blue.svg)
![TypeScript](https://img.shields.io/badge/TypeScript-6.0.2-blue.svg)
![Material--UI](https://img.shields.io/badge/Material--UI-9.0.1-blue.svg)
![Vite](https://img.shields.io/badge/Vite-8.0.10-yellow.svg)

## ✨ Features

### 🚀 **Modern Tech Stack**

- **React 19** with latest features and optimizations
- **TypeScript** for type safety and better developer experience
- **Vite** for lightning-fast development and building
- **Material-UI (MUI)** for beautiful, accessible components

### 📱 **Mobile-First Design**

- **Mobile-Only**: Optimized exclusively for mobile devices (≤768px)
- **Touch-Friendly**: All interactive elements meet accessibility standards (44px minimum)
- **Progressive Web App Ready**: Can be deployed as a PWA
- **Dark Theme**: Modern dark theme by default

### 🌍 **Internationalization**

- **Multi-Language Support**: English and Vietnamese
- **Auto-Detection**: Automatically detects browser language
- **Persistent Preferences**: Saves language choice in localStorage
- **Complete Coverage**: All user-facing text is translated

### 🔧 **Developer Experience**

- **TanStack Router**: Type-safe routing with code splitting
- **React Hook Form + Zod**: Robust form validation
- **ESLint + Prettier**: Code quality and formatting
- **Hot Module Replacement**: Instant updates during development

### 🤖 **AI-Powered Development**

- **Copilot Skills**: Custom skills for component and page creation
- **Consistent Architecture**: Automated code generation following project conventions
- **Mobile-Optimized**: AI generates mobile-first components and pages

## 🏗️ Architecture

```
src/
├── components/          # Reusable components (kebab-case directories)
│   ├── component-name/
│   │   ├── index.tsx    # Component file
│   │   └── styles.css   # Optional complex styles
├── pages/              # Page components
│   ├── page-name/
│   │   ├── index.tsx   # Page file
│   │   └── styles.css  # Optional complex styles
├── routes/             # Route definitions
│   ├── page-name.tsx   # Route files
├── locales/            # i18n translations
│   ├── en.json         # English translations
│   └── vi.json         # Vietnamese translations
├── schemas/            # Zod validation schemas
├── config/             # Configuration files
│   └── i18n.ts         # i18n setup
└── .agents/            # Copilot skills
    └── skills/
        ├── component-creation/
        └── page-creation/
```

## 🚀 Getting Started

### Prerequisites

- Node.js 18+
- npm or pnpm

### Installation

```bash
# Clone the repository
git clone https://github.com/yourusername/aprily-fe.git
cd aprily-fe

# Install dependencies
pnpm install

# Start development server
pnpm dev

# Build for production
pnpm build

# Preview production build
pnpm preview
```

### Available Scripts

- `pnpm dev` - Start development server
- `pnpm build` - Build for production
- `pnpm preview` - Preview production build
- `pnpm lint` - Run ESLint

## 📱 Pages

- **Home** (`/`) - Landing page
- **Sign In** (`/sign-in`) - User authentication
- **Sign Up** (`/sign-up`) - User registration

## 🎨 Design System

### Components

- **MobileOnly**: Restricts app to mobile devices only
- **LanguageSwitcher**: Toggle between English and Vietnamese
- **Form Components**: Sign-in and sign-up forms with validation

### Styling

- **Material-UI**: Pre-built components with customization
- **CSS Modules**: For complex component-specific styles
- **Responsive**: Mobile-first responsive design
- **Dark Theme**: Consistent dark theme across the app

## 🌍 Internationalization

The app supports two languages:

- **English** (en)
- **Vietnamese** (vi)

Language switching is available in the top-right corner of the app.

### Adding New Translations

1. Add keys to `src/locales/en.json` and `src/locales/vi.json`
2. Use the `useTranslation` hook in components:

```tsx
import { useTranslation } from "react-i18next";

export const MyComponent = () => {
  const { t } = useTranslation();

  return <div>{t("my.key")}</div>;
};
```

## 🤖 Copilot Integration

This project includes custom Copilot skills for automated code generation:

### Component Creation Skill

- Generates components following project conventions
- Creates proper directory structure (`kebab-case/index.tsx`)
- Includes i18n support and TypeScript interfaces

### Page Creation Skill

- Creates pages with routing setup
- Generates route files and page components
- Follows mobile-first design principles

## 📦 Tech Stack Details

- **Frontend Framework**: React 19
- **Language**: TypeScript
- **Build Tool**: Vite
- **UI Library**: Material-UI v9
- **Routing**: TanStack Router v1
- **Forms**: React Hook Form + Zod
- **i18n**: i18next + react-i18next
- **Linting**: ESLint
- **Formatting**: Prettier

## 🎯 Project Goals

- **Mobile-First**: Optimized for mobile user experience
- **Performance**: Fast loading and smooth interactions
- **Accessibility**: WCAG compliant touch targets and navigation
- **Developer Experience**: Modern tooling and AI-assisted development
- **Scalability**: Modular architecture for easy expansion

## 📄 License

This project is private and proprietary.

## 🤝 Contributing

This is a private project. Contributions are welcome through pull requests.

## 📞 Contact

For questions or support, please contact the development team.
import reactDom from 'eslint-plugin-react-dom'

export default defineConfig([
globalIgnores(['dist']),
{
files: ['**/*.{ts,tsx}'],
extends: [
// Other configs...
// Enable lint rules for React
reactX.configs['recommended-typescript'],
// Enable lint rules for React DOM
reactDom.configs.recommended,
],
languageOptions: {
parserOptions: {
project: ['./tsconfig.node.json', './tsconfig.app.json'],
tsconfigRootDir: import.meta.dirname,
},
// other options...
},
},
])

```

```
