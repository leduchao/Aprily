import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/_public/help/")({
  component: RouteComponent,
});

function RouteComponent() {
  return <div>Hello "/help/"!</div>;
}
