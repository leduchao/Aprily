import { useNavigate } from "@tanstack/react-router";
import type { Thread } from "../../../../data/chat";
import { Avatar } from "../../../common/avatar";

type Props = {
  thread: Thread;
};

export const ThreadDetailsHeader = ({ thread }: Props) => {
  const navigate = useNavigate();

  return (
    <header className="d-flex justify-content-between align-items-center p-3 bg-body">
      <div className="d-flex align-items-center gap-2 min-w-0">
        <button
          type="button"
          className="btn btn-link text-body p-0 d-flex align-items-center justify-content-center"
          onClick={() => navigate({ to: "/" })}
          aria-label="Back"
          style={{ width: 40, height: 40 }}
        >
          <i className="bi bi-chevron-left fs-5"></i>
        </button>

        <div className="d-flex align-items-center gap-2 min-w-0">
          <Avatar name={thread.title} size={40} />

          <h5 className="fw-bold mb-0 text-truncate">{thread.title}</h5>
        </div>
      </div>

      <div className="d-flex align-items-center gap-1">
        <button
          type="button"
          className="btn btn-link text-body p-0 d-flex align-items-center justify-content-center"
          aria-label="Video call"
          style={{ width: 40, height: 40 }}
        >
          <i className="bi bi-camera-video fs-5"></i>
        </button>

        <button
          type="button"
          className="btn btn-link text-body p-0 d-flex align-items-center justify-content-center"
          aria-label="Phone call"
          style={{ width: 40, height: 40 }}
        >
          <i className="bi bi-telephone fs-5"></i>
        </button>
      </div>
    </header>
  );
};
