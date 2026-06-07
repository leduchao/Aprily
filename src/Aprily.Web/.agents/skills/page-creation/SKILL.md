---
name: page-creation
description: Guidelines for creating new pages with routing and proper directory structure
type: skill
---

# Page Creation Skill

## Overview

This skill defines the standard structure and conventions for creating React pages with routing in the Aprily frontend project using TanStack Router.

## Page Structure

When creating a new page, keep the route definition and page component together inside a dedicated route folder:

```
src/routes/
├── page-name/
│   └── index.tsx          (route definition + page component)
```

## Step-by-Step Guide

### Step 1: Create the Route Folder and Component

Create the route and page component in `src/routes/page-name/index.tsx`:

```tsx
import { Box } from "@mui/material";
import { createFileRoute } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";

export const Route = createFileRoute("/page-name")({
  component: RouteComponent,
});

function RouteComponent() {
  const { t } = useTranslation();

  return <Box>{/* page JSX */}</Box>;
}
```

- Use a route path without a trailing slash, e.g. `/sign-up`, `/threads/$threadId`.

## Naming Conventions

### Route Folder Name

- Use **kebab-case** for the folder name (e.g., `user-profile`, `settings-page`)
- Inside that folder, create `index.tsx` for the route definition and page component
- Match the URL path by folder name (e.g., route folder `user-profile` → `/user-profile`)
- For nested routes, use folder nesting or file naming conventions supported by TanStack Router

### Page Component Export

- Use the standard route component name `RouteComponent` inside `src/routes/<page-name>/index.tsx`
- Keep it internal to the route file (no export needed)

## Complete Example: Creating a Dashboard Page

### 1. Create Route Folder and Component

`src/routes/dashboard/index.tsx`:

```tsx
import { Box, Container, Typography, Grid, Card } from "@mui/material";
import { createFileRoute } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";

export const Route = createFileRoute("/dashboard")({
  component: RouteComponent,
});

function RouteComponent() {
  const { t } = useTranslation();

  return (
    <Container maxWidth="lg">
      <Box sx={{ py: 4 }}>
        <Typography variant="h3" sx={{ mb: 4 }}>
          {t("dashboard.title")}
        </Typography>

        <Grid container spacing={2}>
          <Grid item xs={12} sm={6} md={4}>
            <Card>{/* card content */}</Card>
          </Grid>
        </Grid>
      </Box>
    </Container>
  );
}
```

### 3. Add i18n Translations

In `src/locales/en.json`:

```json
{
  "dashboard": {
    "title": "Dashboard"
  }
}
```

## Implementation Rules

### ✅ DO

- Create a route folder in `src/routes/` with kebab-case name
- Place route definition and page component together in `src/routes/<page-name>/index.tsx`
- Use i18n for all user-visible text
- Add TypeScript types for page props, params or loaders as needed
- Make pages responsive using Material-UI sx props
- Design pages for mobile-first (single column, touch-friendly)
- Use Material-UI mobile breakpoints only (xs, sm)
- Optimize layouts for vertical scrolling
- Ensure all interactive elements are touch-friendly (44px minimum)

### ❌ DON'T

- Create route files in other locations
- Name page directories differently from route files
- Use PascalCase for directory names
- Create multiple pages in one file
- Use default exports for page components
- Hardcode text strings
- Skip i18n translations
- Make page directories without corresponding route files
- Design for desktop/tablet layouts
- Use horizontal scrolling
- Create touch targets smaller than 44px

## Nested Routes (Optional)

For nested pages like `/settings/profile`, use nested route folders:

`src/routes/settings/profile/index.tsx`:

```tsx
import { Box } from "@mui/material";
import { createFileRoute } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";

export const Route = createFileRoute("/settings/profile")({
  component: RouteComponent,
});

function RouteComponent() {
  const { t } = useTranslation();

  return <Box>{/* profile page JSX */}</Box>;
}
```

## Layout Structure

Recommended page layout structure (mobile-first):

```tsx
import { Box, Container, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";

function RouteComponent() {
  const { t } = useTranslation();

  return (
    <Container maxWidth="sm">
      {" "}
      {/* Mobile-first: maxWidth="sm" */}
      <Box sx={{ py: 4 }}>
        {/* Header Section */}
        <Box sx={{ mb: 4 }}>
          <Typography variant="h3" sx={{ fontWeight: "700", mb: 1 }}>
            {t("page.title")}
          </Typography>
          <Typography variant="body1" color="text.secondary">
            {t("page.subtitle")}
          </Typography>
        </Box>

        {/* Content Section - Single Column for Mobile */}
        <Box sx={{ mt: 4 }}>
          {/* main content - design for vertical scrolling */}
        </Box>
      </Box>
    </Container>
  );
}
```

## File Organization

```
src/
├── routes/
│   ├── __root.tsx
│   ├── index.tsx
│   ├── sign-up/
│   │   └── index.tsx         ← route + page component
│   ├── dashboard/
│   │   └── index.tsx         ← route + page component
│   └── settings/
│       └── profile/
│           └── index.tsx     ← nested route + page component
```

## Translation Keys Pattern

For page translations, use this key structure in `src/locales/*.json`:

```json
{
  "page": {
    "title": "Page Title",
    "subtitle": "Page Subtitle",
    "field1": "Field Label",
    "button": "Button Text"
  }
}
```

## Benefits

- 📁 **Consistency**: Uniform structure for pages across the project
- 🔀 **Routing**: Clear route definitions alongside page components
- 🎯 **Organization**: Pages and routes are logically separated but paired
- 🌍 **i18n Ready**: Built-in internationalization support
- 📱 **Mobile-First**: Optimized for mobile devices with touch-friendly interfaces
- 📝 **Type Safe**: Full TypeScript support with TanStack Router
- 👆 **Touch Optimized**: All interactive elements designed for mobile touch interaction
