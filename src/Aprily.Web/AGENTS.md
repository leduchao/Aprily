# Aprily Frontend Agents

This file defines the custom agents available for the Aprily frontend project.

## Agents

### Component Creator Agent

**Name:** `component-creator`
**Description:** Specialized agent for creating React components following project conventions
**Skills:**

- `component-creation` - Guidelines for creating new components with proper directory structure and naming conventions

**Usage:**

- Create new React components
- Follow kebab-case directory naming
- Use index.tsx pattern
- Include i18n support
- Add TypeScript interfaces

### Page Creator Agent

**Name:** `page-creator`
**Description:** Specialized agent for creating new pages with routing and proper directory structure
**Skills:**

- `page-creation` - Guidelines for creating new pages with routing and proper directory structure

**Usage:**

- Create new pages with TanStack Router
- Set up route definitions
- Create page components in pages/ directory
- Include i18n translations
- Follow responsive design patterns

## Skills

### component-creation

**Path:** `.agents/skills/component-creation/SKILL.md`
**Description:** Guidelines for creating new components with proper directory structure and naming conventions
**Type:** skill

### page-creation

**Path:** `.agents/skills/page-creation/SKILL.md`
**Description:** Guidelines for creating new pages with routing and proper directory structure
**Type:** skill

## Agent Configuration

### Default Agent

The default agent has access to all skills and can perform general coding tasks.

### Specialized Agents

- Use `component-creator` when specifically asked to create components
- Use `page-creator` when specifically asked to create pages
- Use default agent for general coding, debugging, or refactoring tasks

## Usage Examples

### Creating a Component

```
Create a UserCard component
```

→ Uses `component-creator` agent

### Creating a Page

```
Create a dashboard page
```

→ Uses `page-creator` agent

### General Tasks

```
Fix this bug in the sign-in form
```

→ Uses default agent

## Project Context

This project uses:

- **React 19** with TypeScript
- **Material-UI (MUI)** for components
- **TanStack Router** for routing
- **React Hook Form** with Zod validation
- **i18next** for internationalization (English/Vietnamese)
- **Vite** for build tooling

### Application Type

- **Mobile-Only Application**: Designed exclusively for mobile devices (≤ 768px viewport)
- **No Desktop Support**: Does not support desktop or tablet layouts
- **Progressive Web App Ready**: Optimized for mobile PWA deployment

## File Structure

```
src/
├── components/          # Reusable components
│   ├── component-name/  # Kebab-case directories
│   │   ├── index.tsx    # Component files
│   │   └── styles.css   # Optional CSS file for complex styles
├── pages/              # Page components
│   ├── page-name/      # Kebab-case directories
│   │   ├── index.tsx   # Page files
│   │   └── styles.css  # Optional CSS file for complex styles
├── routes/             # Route definitions
│   ├── page-name.tsx   # Route files
├── locales/            # i18n translations
│   ├── en.json
│   └── vi.json
├── schemas/            # Zod validation schemas
└── config/             # Configuration files
    └── i18n.ts         # i18n setup
```

## Coding Standards

### Components

- Directory: `kebab-case`
- File: `index.tsx`
- Export: `export const PascalCase = () => {}`
- Props: TypeScript interfaces
- i18n: Use `useTranslation()` hook

### Pages

- Route file: `routes/kebab-case.tsx`
- Page directory: `pages/kebab-case/`
- Page file: `pages/kebab-case/index.tsx`
- Export: `export function PascalCase() {}`
- i18n: Include translations in locale files

### Styling

- Use Material-UI sx prop for simple styles
- For complex CSS, create a separate CSS file in the component/page folder
- CSS file naming: `styles.css` or `component-name.css`
- Import CSS files at the top of component files
- Mobile-first responsive design (no desktop breakpoints)
- Dark theme by default
- Consistent spacing and typography optimized for mobile

## Design Guidelines

### Mobile-First Design

- **Mobile Only**: This application is designed exclusively for mobile devices
- **No Desktop Support**: Do not create desktop-specific layouts or responsive breakpoints for large screens
- **Mobile Breakpoints**: Use Material-UI's mobile breakpoints only (xs, sm)
- **Viewport**: Target mobile viewport width ≤ 768px
- **Touch Interactions**: Design for touch gestures and mobile interactions

### Mobile UX Patterns

- **Single Column Layout**: Use single column layouts optimized for mobile
- **Thumb-Friendly**: Ensure buttons and interactive elements are touch-friendly (minimum 44px)
- **Vertical Scrolling**: Design for vertical scrolling, avoid horizontal scrolling
- **Bottom Navigation**: Consider bottom navigation for mobile apps
- **Swipe Gestures**: Implement swipe gestures where appropriate

### Component Design

- **Mobile Cards**: Use card-based layouts for content organization
- **Compact Forms**: Design forms for mobile input (keyboard, autocomplete)
- **Modal Sheets**: Use bottom sheets instead of desktop modals
- **Floating Action Buttons**: Use FABs for primary actions on mobile

### Typography & Spacing

- **Readable Font Sizes**: Use appropriate font sizes for mobile reading
- **Touch Spacing**: Ensure adequate spacing between interactive elements
- **Mobile Typography Scale**: Use Material-UI's mobile-optimized typography

### Performance

- **Mobile Performance**: Optimize for mobile network conditions
- **Lazy Loading**: Implement lazy loading for images and content
- **Bundle Size**: Keep bundle size minimal for mobile networks
