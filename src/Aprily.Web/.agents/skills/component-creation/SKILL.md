---
name: component-creation
description: Guidelines for creating new components with proper directory structure and naming conventions
type: skill
---

# Component Creation Skill

## Overview

This skill defines the standard structure and conventions for creating React components in the Aprily frontend project.

## Component Structure

When creating a new component, always follow this structure:

```
src/components/
├── ComponentName/          (kebab-case directory name)
│   └── index.tsx          (main component file)
```

## Naming Conventions

### Directory Name

- Use **kebab-case** (lowercase with hyphens)
- Examples: `user-profile`, `language-switcher`, `mobile-only`, `auth-form`

### Component Export

- Use **PascalCase** (UpperCamelCase)
- Match the directory name conceptually
- Examples: `UserProfile`, `LanguageSwitcher`, `MobileOnly`, `AuthForm`

### File Name

- Always use `index.tsx` inside the component directory
- Never create multiple components in one file

## Code Template

```tsx
import { Box } from "@mui/material";
import { useTranslation } from "react-i18next";

interface ComponentNameProps {
  // Define props here
}

export const ComponentName = ({} /* props */ : ComponentNameProps) => {
  const { t } = useTranslation();

  return <Box>{/* component JSX */}</Box>;
};
```

## Implementation Rules

### ✅ DO

- Create component in a subdirectory with kebab-case name
- Use `index.tsx` as the filename
- Use PascalCase for component function name
- Use i18n (`useTranslation()`) for all user-visible text
- Add TypeScript interfaces for component props
- Export component as named export (`export const ComponentName`)
- Design components for mobile-first (touch-friendly, single column)
- Use Material-UI mobile breakpoints only (xs, sm)
- Ensure touch targets are at least 44px
- Optimize for vertical scrolling layouts

### ❌ DON'T

- Place components directly in `src/components/` without a subdirectory
- Create multiple components in one file
- Name the file anything other than `index.tsx`
- Use hardcoded strings for UI text
- Use default exports
- Create files like `Component.tsx` or `component.tsx`
- Design for desktop/tablet layouts
- Use horizontal scrolling
- Create touch targets smaller than 44px

## Import Pattern

When importing a component:

```tsx
import { UserProfile } from "../components/user-profile";
import { LanguageSwitcher } from "../components/language-switcher";
```

## Translation (i18n)

All components should use i18n for user-visible strings:

```tsx
import { useTranslation } from "react-i18next";

export const MyComponent = () => {
  const { t } = useTranslation();

  return <button>{t("common.submit")}</button>;
};
```

## Example: Creating a UserCard Component

1. Create directory: `src/components/user-card/`
2. Create file: `src/components/user-card/index.tsx`
3. Implement component:

```tsx
import { Box, Card, CardContent, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";

interface UserCardProps {
  name: string;
  email: string;
}

export const UserCard = ({ name, email }: UserCardProps) => {
  const { t } = useTranslation();

  return (
    <Card>
      <CardContent>
        <Typography variant="h5">{name}</Typography>
        <Typography variant="body2" color="textSecondary">
          {email}
        </Typography>
      </CardContent>
    </Card>
  );
};
```

4. Import elsewhere:

```tsx
import { UserCard } from "../components/user-card";
```

## Benefits

- 📁 **Consistency**: Uniform directory structure across the project
- 🧩 **Modularity**: Each component is self-contained
- 🚀 **Scalability**: Easy to find and maintain components
- 🎯 **Clarity**: Clear naming conventions for quick identification
- 🌍 **Internationalization**: Built-in support for multiple languages
- 📝 **Type Safety**: TypeScript ensures prop validation
- 📱 **Mobile-First**: Components optimized for mobile touch interaction
- 👆 **Touch-Friendly**: All interactive elements meet mobile accessibility standards
