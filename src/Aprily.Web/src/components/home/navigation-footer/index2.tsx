import "./index.css";
import { Link } from "@tanstack/react-router";

const navItems = [
  {
    to: "/",
    icon: <i className="bi bi-chat-fill"></i>,
  },
  {
    to: "/notifications",
    icon: <i className="bi bi-bell-fill"></i>,
  },
  {
    to: "/menu",
    icon: <i className="bi bi-list icon-list"></i>,
  },
];

export const NavigationFooter = () => {
  return (
    <footer className="p-3 d-flex justify-content-center">
      <nav className="d-flex justify-content-evenly align-items-center bg-primary rounded-pill py-2 navbar">
        {navItems.map(({ to, icon }) => (
          <Link
            key={to}
            to={to}
            className="text-white d-flex align-items-center justify-content-center text-decoration-none fs-5"
          >
            {icon}
          </Link>
        ))}
      </nav>
    </footer>
  );
};
