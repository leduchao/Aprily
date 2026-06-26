FROM node:24-alpine AS build
WORKDIR /app
RUN corepack enable
COPY src/Aprily.Client/package.json src/Aprily.Client/pnpm-lock.yaml src/Aprily.Client/pnpm-workspace.yaml ./
RUN pnpm install --frozen-lockfile
COPY src/Aprily.Client/ ./
RUN pnpm build

FROM nginx:1.29-alpine AS final
COPY src/Aprily.Client/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
