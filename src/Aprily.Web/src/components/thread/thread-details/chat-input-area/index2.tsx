import { useRef, type ChangeEvent, type KeyboardEvent } from "react";
import "./index.css";

type Props = {
  value: string;
  onChange: (value: string) => void;
  onSend: () => void;
  onUpload?: (files: File[]) => void;
};

export const ChatInputArea = ({ value, onChange, onSend, onUpload }: Props) => {
  const fileInputRef = useRef<HTMLInputElement | null>(null);
  const disabled = !value.trim();

  const handleUpload = (event: ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(event.target.files ?? []);

    if (files.length > 0) {
      onUpload?.(files);
    }

    event.target.value = "";
  };

  const handleKeyDown = (event: KeyboardEvent<HTMLTextAreaElement>) => {
    if (event.key === "Enter" && !event.shiftKey && !disabled) {
      event.preventDefault();
      onSend();
    }
  };

  return (
    <footer className="chat-input-footer">
      <input
        ref={fileInputRef}
        type="file"
        multiple
        hidden
        onChange={handleUpload}
      />

      <div className="chat-input-container">
        <button
          type="button"
          className="chat-inner-btn"
          onClick={() => fileInputRef.current?.click()}
          aria-label="Upload files"
        >
          <i className="bi bi-plus-lg"></i>
        </button>

        <button type="button" className="chat-inner-btn" aria-label="Emoji">
          <i className="bi bi-emoji-smile"></i>
        </button>

        <button
          type="button"
          className="chat-inner-btn"
          aria-label="Voice message"
        >
          <i className="bi bi-mic"></i>
        </button>

        <div className="chat-input-box">
          <textarea
            rows={1}
            className="form-control rounded-pill chat-textarea"
            placeholder="Enter message..."
            value={value}
            onChange={(event) => onChange(event.target.value)}
            onKeyDown={handleKeyDown}
          />
        </div>
        <button
          type="button"
          className={`chat-send-btn ${disabled ? "disabled" : "active"}`}
          onClick={onSend}
          disabled={disabled}
          aria-label="Send message"
        >
          <i className="bi bi-send chat-send-icon"></i>
        </button>
      </div>
    </footer>
  );
};
