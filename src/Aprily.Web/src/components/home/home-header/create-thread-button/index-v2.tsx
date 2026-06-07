import "./index.css";

export const CreateThreadButton = () => {
  return (
    <div className="dropdown">
      <button
        type="button"
        className="btn btn-outline-primary text-white rounded-pill fw-bold text-uppercase dropdown-toggle"
        data-bs-toggle="dropdown"
        aria-expanded="false"
      >
        <i className="bi bi-plus-lg me-2 plus-icon"></i>
        create
      </button>

      <ul className="dropdown-menu dropdown-menu-end">
        <li>
          <button
            type="button"
            className="dropdown-item d-flex align-items-center"
          >
            <i className="bi bi-chat-left-text me-2"></i>
            Chat
          </button>
        </li>

        <li>
          <button
            type="button"
            className="dropdown-item d-flex align-items-center"
          >
            <i className="bi bi-people me-2"></i>
            Group
          </button>
        </li>
      </ul>
    </div>
  );
};
